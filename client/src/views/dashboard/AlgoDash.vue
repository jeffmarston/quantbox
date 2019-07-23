<template>
  <div class="animated fadeIn dashboard">
    <div class="top-bar">
      <b-row>
        <b-col md="12" lg="12">
          <h4>Trading Algorithms</h4>
          <b-button class="top-bar-button" variant="primary" @click="createNewAlgo">
            <i class="fa fa-plus"></i>Create New
          </b-button>
          <b-button class="top-bar-button" variant="secondary" @click="openConsole">
            <i class="fa fa-code"></i>Console
          </b-button>
        </b-col>
      </b-row>
    </div>

    <div class="card-view">
      <b-row>
        <b-col
          class="algo-column"
          :class="{ 'large-card': largecard}"
          v-for="(algo, idx) in allAlgos"
          :key="idx"
        >
          <algo-card
            :algo="algo"
            :parentSize="size"
            @showParameters="showParameters(algo)"
            @showCode="showCode(algo)"
            @deleteAlgo="deleteAlgo(algo)"
          ></algo-card>
        </b-col>
      </b-row>
    </div>

    <b-modal
      no-fade
      v-if="showModal"
      :title="editOptions.create ? 'Create' : 'Edit'"
      size="xl"
      v-model="showModal"
      @ok="saveModal"
    >
      <b-tabs>
        <b-tab :active="editOptions.focus === 'code'" title="Code">
          <code-editor></code-editor>
        </b-tab>
        <b-tab :active="editOptions.focus === 'parameters'" title="Parameters">
          <algo-config ref="configComponent" :algoName="editingAlgo.name" :options="editOptions"></algo-config>
        </b-tab>
      </b-tabs>
    </b-modal>
  </div>
</template>

<script>
import AlgoCard from "./AlgoCard";
import SignalrHub from "../../shared/SignalrHub";
import { getAlgos } from "../../shared/restProvider";
import CodeEditor from "./CodeEditor";
import AlgoConfig from "./AlgoConfig";

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
      largecard: false,
      size: 0,
      allAlgos: [],
      showModal: false,
      editingAlgo: null,
      editOptions: { create: true }
    };
  },
  methods: {
    resize(newSize) {
      this.largecard = this.$el.clientWidth < 1300;
    },
    resized(newSize) {
      this.size = this.$el.clientWidth;
    },
    openConsole() {
      this.$emit("toggleConsole");
    },
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
            algoToUpdate.stats = updatedAlgo.stats;
            algoToUpdate.history = [];

            // transform into chart datapoints
            updatedAlgo.history.forEach(dp => {
              algoToUpdate.history.push({
                date: dp.date,
                value: dp.value
              });
            });

            this.allAlgos[i] = algoToUpdate;
            //console.log(algoToUpdate);
          }
        }
        // Inserting a new card
        if (!foundIt) {
          this.load();
        }
      });

      conn.on("algo-stats", (name, stats) => {
        
        console.log(name + ": " + JSON.stringify(stats));

        for (let i = 0; i < this.allAlgos.length; i++) {
          if (this.allAlgos[i].name === name) {
            console.log(stats);
            this.allAlgos[i].stats = stats;
          }
        }
      });

      conn.on("delete-algo", algoName => {
        let idxToRemove = -1;
        for (let i = 0; i < this.allAlgos.length; i++) {
          if (this.allAlgos[i].name === algoName) {
            idxToRemove = i;
          }
        }
        if (idxToRemove >= 0) {
          this.allAlgos.splice(idxToRemove, 1);
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
      let newAlgo = {};
      this.editingAlgo = newAlgo;
      this.editOptions = { create: true, focus: "code" };
      this.showModal = !this.showModal;
    },
    showParameters(algo) {
      this.editingAlgo = algo;
      this.editOptions = { create: false, focus: "parameters" };
      this.showModal = !this.showModal;
    },
    showCode(algo) {
      this.editingAlgo = algo;
      this.editOptions = { create: false, focus: "code" };
      this.showModal = !this.showModal;
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
.fa {
  margin-right: 10px;
}
.top-bar-button {
  float: right;
  margin-left: 8px;
}
.algo-column {
  min-width: 50%;
  flex-grow: 0;
}
.large-card {
  flex: 0 0 100%;
  max-width: 100%;
}
.dashboard {
  min-width: 450px;
  overflow-y: auto;
  height: 100%;
}
</style>
