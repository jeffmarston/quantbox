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
      size="lg"
      v-model="isEditMode"
      @ok="myModal = false"
    >
      <b-tabs>
        <b-tab active title="Code">
          <code-editor></code-editor>
        </b-tab>
        <b-tab title="Parameters">
          <algo-config :algoName="editingAlgo.name"></algo-config>
        </b-tab>
      </b-tabs>
    </b-modal>
  </div>
</template>

<script>
import AlgoCard from "./AlgoCard";
import SignalrHub from "../../shared/SignalrHub";
import { getAlgos } from "../../shared/algoProvider";
import CodeEditor from "../admin/CodeEditor";
import AlgoConfig from "../admin/AlgoConfig";

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
    getAlgos().then(o => {
      o.forEach(algo => {
        algo.lastMsg = "";
        this.allAlgos.push(algo);
        //algo.history = [];
      });
      this.subscribeToChange();
    });
  },
  data: function() {
    return {
      allAlgos: [],
      isEditMode: false,
      editingAlgo: null
    };
  },
  methods: {
    subscribeToChange() {
      conn.on("algos", updatedAlgo => {
        for (let i = 0; i < this.allAlgos.length; i++) {
          if (this.allAlgos[i].name === updatedAlgo.name) {
            let newGuy = this.allAlgos[i];
            newGuy.enabled = updatedAlgo.enabled;
            newGuy.tradesCreated = updatedAlgo.tradesCreated;
            newGuy.history = [];

            // transform into chart datapoints
            updatedAlgo.history.forEach(dp => {
              newGuy.history.push({
                date: new Date(dp.date),
                name: "point_" + dp.date,
                value: dp.value
              });
            });

            this.allAlgos[i] = newGuy;
          }
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
      //this.showEditor = true;
    },
    showOptions(algo) {
      this.editingAlgo = algo;
      this.isEditMode = !this.isEditMode;
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
