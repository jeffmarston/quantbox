<template>
  <div class="animated fadeIn">
    <b-row>
      <b-col xl="12">
        <b-button variant="primary">
          <i class="icon-refresh"/>&nbsp;Restart All
        </b-button>
        <b-button type="button" variant="secondary" @click="myModal = true" class="mr-1">Log setup</b-button>
      </b-col>
    </b-row>
    <b-row>
      <b-col sm="12">
        <ag-grid-vue
          style="height: calc(100vh - 210px)"
          class="ag-theme-balham"
          :columnDefs="columnDefs"
          :rowData="rowData"
          :gridOptions="gridOptions"
          :floatingFilter="true"
          rowSelection="multiple"
          suppressCellSelection
          animate-rows
          @grid-ready="onGridReady"
          @row-selected="onRowSelected"
          :getRowNodeId="getRowNodeId"
          @cell-clicked="cellClick"
        ></ag-grid-vue>
      </b-col>
    </b-row>

    <!-- Modal Component -->
    <b-modal title="Log Setup" v-model="myModal" @ok="myModal = false">
      <b-checkbox v-model="forceAllLogs">Log everything to the same place</b-checkbox>
      <b-form-group label="Log Location" label-for="logLocationInput" label-cols="3">
        <b-form-file id="logLocationInput" directory :disabled="!forceAllLogs"></b-form-file>
      </b-form-group>
    </b-modal>
  </div>
</template>

<script>
const _ = require("lodash");
const env = require("../../environment.config.json");
import SignalrHub from "../../shared/SignalrHub";

let signalrHub = null;
let onReady = function() {};

signalrHub = new SignalrHub(onReady);
let conn = signalrHub.connection;
let today = new Date();
let todaysDate =
  today.getMonth() + 1 + "/" + today.getDate() + "/" + today.getFullYear();

import { AgGridVue } from "ag-grid-vue";
import Router from "vue-router";

function actionCellRendererFunc(params) {
  if (params.data.status === "Stopped") {
    return `<a class="icon-hover-hightlight"><i style="display: inline" class="icon-control-play icons"></i></a>`;
  }
  if (params.data.status === "Running") {
    return `<a class="icon-hover-hightlight"><i style="display: inline" class="icon-control-pause"></i></a>`;
  }
}

function logCellRendererFunc(params) {
  return `<a class="icon-hover-hightlight"><i style="display: inline" class="icon-paper-clip"></i></a>`;
}

export default {
  name: "services",
  components: { AgGridVue },
  data: () => {
    return {
      myModal: false,
      forceAllLogs: false,
      columnDefs: null,
      rowData: null,
      selectedRow: null,
      gridOptions: null
    };
  },
  beforeMount() {
    this.columnDefs = [
      {
        headerName: "Status",
        field: "status",
        sortable: true,
        filter: "agTextColumnFilter",
        resizable: true,
        cellStyle: function(params) {
          return params.data.status == "Stopped"
            ? { color: "white", backgroundColor: "#f64846" }
            : params.data.status == "StartPending" ||
              params.data.status == "StopPending"
            ? { color: "black", backgroundColor: "#ffc107" }
            : params.data.status == "Running"
            ? { color: "white", backgroundColor: "#4dbd74" }
            : null;
        }
      },
      {
        headerName: "Action",
        pinned: true,
        width: 40,
        cellRenderer: actionCellRendererFunc
      },
      {
        headerName: "Logs",
        pinned: true,
        width: 34,
        cellRenderer: logCellRendererFunc
      },
      {
        headerName: "Server",
        field: "server",
        sortable: true,
        filter: "agTextColumnFilter",
        resizable: true
      },
      {
        headerName: "Name",
        field: "name",
        sortable: true,
        filter: "agTextColumnFilter",
        floatingFilterComponentParams: {
          debounceMs: 1000
        },
        resizable: true
      },
      {
        headerName: "Uptime",
        field: "uptime",
        sortable: true,
        filter: "agTextColumnFilter",
        resizable: true
      },
      {
        headerName: "FileName",
        field: "filename",
        sortable: true,
        filter: "agTextColumnFilter",
        resizable: true
      },
      {
        headerName: "StartMode",
        field: "startMode",
        sortable: true,
        filter: "agTextColumnFilter",
        resizable: true
      },
      {
        headerName: "Start Name",
        field: "startName",
        sortable: true,
        filter: "agTextColumnFilter",
        resizable: true
      },
      {
        headerName: "Path",
        field: "path",
        sortable: true,
        filter: "agTextColumnFilter",
        resizable: true
      }
    ];

    this.rowData = [];
    this.gridOptions = {};
  },
  methods: {
    cellClick(event) {
    },
    onGridReady(params) {
      this.gridApi = params.api;
      this.columnApi = params.columnApi;
      this.gridOptions.onRowDoubleClicked = this.onDoubleClick;
      this.populateGrid();
      this.subscribeToServiceChange();
    },
    subscribeToServiceChange() {
      conn.on("service", (machineName, svcName, svcData) => {
        this.gridApi.forEachNodeAfterFilter(row => {
          if (row.data.name === svcName) {
            row.data = svcData;
            this.gridApi.redrawRows(row);
          }
        });
      });
    },
    populateGrid() {
        // navTreeData.servers.forEach(svr => {
        //   fetch(env.serverAddress + "/api/algo, {
        //     mode: "cors"
        //   }).then(response => {
        //     if (response.status !== 200) {
        //       console.error("!! Status Code: " + response.status);
        //       console.error(response);
        //       return;
        //     }
        //     // Examine the text in the response
        //     response.json().then(data => {
        //       data.forEach(service => {
        //         service.server = svr.name;
        //         this.rowData.push(service);
        //       });
        //     });
        //   });
        // });
    },
    disableButton(row, buttonType) {
      if (!row) return true;
      if (buttonType === "start") {
        return row.status === "Running";
      } else if (buttonType === "stop") {
        return row.status !== "Running";
      } else if (buttonType === "logs") {
        return false;
      }
    },
    onRowSelected() {
      this.selectedRow = this.gridApi.getSelectedRows()[0];
    },
    onDoubleClick() {
      if (this.selectedRow.status === "Running") {
        this.stopSvc();
      } else {
        this.startSvc();
      }
    },
    getRowNodeId(data) {
      return data.name + "|" + data.server;
    },
    startSvc() {
      fetch(
        env.serverAddress +
          "/api/services/" +
          this.selectedRow.server +
          "/start/" +
          this.selectedRow.name,
        {
          method: "POST",
          mode: "cors",
          cache: "no-cache",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify(this.selectedRow)
        }
      );
    },
    stopSvc() {
      fetch(
        env.serverAddress +
          "/api/services/" +
          this.$route.params.name +
          "/stop/" +
          this.selectedRow.name,
        {
          method: "POST",
          mode: "cors",
          cache: "no-cache",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify(this.selectedRow)
        }
      );
    }
  }
};
</script>
