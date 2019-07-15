<template>
  <div class="animated fadeIn">
    <div class="top-bar">
      <b-row>
        <b-col md="12" lg="12">
          <h4>Trading Algorithms</h4>
          <b-button class="top-bar-button" variant="primary" @click="createNewAlgo">
            <i class="fa fa-plus"></i>Create New
          </b-button>
        </b-col>
      </b-row>
    </div>

    <div class="card-view">
      <b-row>
        <b-col lg="6" xs="12" v-for="(algo, idx) in allAlgos" :key="idx">
          <algo-card :algo="algo" @showOptions="showOptions(algo)"></algo-card>
        </b-col>
      </b-row>
    </div>

    <b-modal
      v-if="isEditMode"
      :title="editingAlgo.name"
      size="xl"
      v-model="isEditMode"
      @ok="saveModal"
    >
      <b-tabs>
        <b-tab title="Code">
          <code-editor></code-editor>
        </b-tab>
        <b-tab active title="Parameters">
          <algo-config ref="configComponent" :algoName="editingAlgo.name"></algo-config>
        </b-tab>
      </b-tabs>
    </b-modal>
  </div>
</template>

<script>
import AlgoCard from "./AlgoCard";
import SignalrHub from "../../shared/SignalrHub";
import { getAlgos } from "../../shared/restProvider";
import CodeEditor from "../admin/CodeEditor";
import AlgoConfig from "../admin/AlgoConfig";
import { load } from "@amcharts/amcharts4/.internal/core/utils/Net";

const _ = require("lodash");

let signalrHub = null;
let onReady = function() {};
signalrHub = new SignalrHub(onReady);
let conn = signalrHub.connection;

export default {
  name: "algo-dash",
  components: {
    AlgoCard,
    CodeEditor,
    AlgoConfig
  },
  mounted() {
    this.load();
  },
  data: function() {
    return {
      allAlgos: [],
      isEditMode: false,
      editingAlgo: null
    };
  },
  methods: {
    load() {
      getAlgos().then(o => {
        this.allAlgos = [];
        o.forEach(algo => {
          algo.lastMsg = "";
          this.allAlgos.push(algo);
        });
        this.subscribeToChange();
      });
    },
    subscribeToChange() {
      conn.on("algos", updatedAlgo => {
        let foundIt = false;
        for (let i = 0; i < this.allAlgos.length; i++) {
          if (this.allAlgos[i].name === updatedAlgo.name) {
            foundIt = true;
            let algoToUpdate = this.allAlgos[i];
            algoToUpdate.enabled = updatedAlgo.enabled;
            algoToUpdate.tradesCreated = updatedAlgo.tradesCreated;
            algoToUpdate.history = [];

            // transform into chart datapoints
            updatedAlgo.history.forEach(dp => {
              algoToUpdate.history.push({
                date: new Date(dp.date),
                name: "point_" + dp.date,
                value: dp.value
              });
            });

            this.allAlgos[i] = algoToUpdate;
          }
        }
        if (!foundIt) {
          let algoToInsert = updatedAlgo;
          algoToInsert.enabled = updatedAlgo.enabled;
          algoToInsert.tradesCreated = updatedAlgo.tradesCreated;

          // transform into chart datapoints
          algoToInsert.history = [];
          updatedAlgo.history.forEach(dp => {
            algoToInsert.history.push({
              date: new Date(dp.date),
              name: "point_" + dp.date,
              value: dp.value
            });
          });

          this.allAlgos.push(algoToInsert);
        }
      });

      conn.on("console", (msg, algoName) => {
        for (let i = 0; i < this.allAlgos.length; i++) {
          if (this.allAlgos[i].name === algoName) {
            let newGuy = this.allAlgos[i];
            newGuy.lastMsg = "[" + new Date().toLocaleTimeString() + "] " + msg;
            this.allAlgos[i] = newGuy;
          }
        }
      });
    },
    createNewAlgo() {
      let newAlgo = { name: "New Algorithm" };
      this.editingAlgo = newAlgo;
      this.isEditMode = !this.isEditMode;
    },
    showOptions(algo) {
      this.editingAlgo = algo;
      this.isEditMode = !this.isEditMode;
    },
    saveModal() {
      this.$refs.configComponent.save();
      this.editingAlgo = {};
      this.load();
    }
  }
};
</script>

<style scoped>
h4 {
  display: inline-block;
  margin-top: 7px;
}
.top-bar {
  margin: 8px 0;
}
.fa-plus {
  margin-right: 10px;
}
.top-bar-button {
  float: right;
}
</style>
