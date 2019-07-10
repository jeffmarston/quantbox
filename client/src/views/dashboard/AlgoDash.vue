<template>
  <div class="animated fadeIn">
    <div class="left-half">
      <b-row>
        <b-col md="12" lg="6" v-for="(algo, idx) in allAlgos" v-bind:key="idx">
          <amChart style="height: 250px;" v-bind:algo="algo"></amChart>
        </b-col>
      </b-row>
    </div>

    <!-- <div class="right-half">
      Hello worldly world
    </div>-->
  </div>
</template>

<script>
import amChart from "./amChart";
import SignalrHub from "../../shared/SignalrHub";
import { Callout, Switch as cSwitch } from "@coreui/vue";
import { getAlgos, enableAlgos } from "../../shared/algoProvider";
import { match } from "minimatch";
import { setInterval } from "timers";

const _ = require("lodash");

let signalrHub = null;
let onReady = function() {
  try {
    signalrHub.subscribe("localhost", "");
  } catch (e) {
    console.error("Failed to subscribe to SignalR updates: ");
    console.error(e);
  }
};

signalrHub = new SignalrHub(onReady);
let conn = signalrHub.connection;

export default {
  name: "algo-dash",
  components: {
    amChart,
    cSwitch
  },
  mounted() {
    getAlgos().then(o => {
      o.forEach(algo => {
        this.allAlgos.push(algo);
        //algo.history = [];
      });
      this.subscribeToChange(this.allAlgos);
    });
  },
  data: function() {
    return {
      allAlgos: []
    };
  },
  methods: {
    subscribeToChange(algos) {
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
    }
  }
};
</script>

<style scoped>
.left-half {
  width: 100%;
  float: left;
}
.right-half {
  width: 0;
  height: 100%;
  background: darkred;
  float: right;
}
</style>
