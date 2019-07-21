<template>
  <b-card>
    <div slot="header">
      <b-row>
        <b-col md="2" class="card-enabled-indicator d-none d-md-block">
          <span v-if="algo.enabled">
            <img class="img-active-state" src="img/ActivePulseAnimated.gif" alt="Active" />
            Enabled
          </span>
          <span v-if="!algo.enabled">
            <img class="img-active-state" src="img/ux-i-offline.svg" alt="Active" />
            Disabled
          </span>
        </b-col>
        <b-col xs="8" class="card-header-block">
          <h5 class="card-title mb-0">{{ algo.name }}</h5>
        </b-col>
        <b-col xs="2">
          <b-button-toolbar class="float-right enable-button-size">
            <b-nav-item-dropdown text="Options" right>
              <b-dropdown-item href="#" @click.prevent="showCode">Code</b-dropdown-item>
              <b-dropdown-item href="#" @click.prevent="showParameters">Parameters</b-dropdown-item>
              <b-dropdown-item href="#" @click.prevent="deleteAlgo">Delete</b-dropdown-item>
            </b-nav-item-dropdown>

            <i v-if="algo.enabled" class="fa fa-toggle-on" @click="toggleEnabled(algo, false)"></i>
            <i v-if="!algo.enabled" class="fa fa-toggle-off" @click="toggleEnabled(algo, true)"></i>
          </b-button-toolbar>
        </b-col>
      </b-row>
    </div>

    <b-row>
      <b-col md="4" xs="11">
        <b-row>
          <div class="card-summary-panel">
            <label class="card-label large">Trades Created</label>
            <h3
              class="trade-count"
              :class="{ 'green-text': algo.enabled }"
            >{{ algo.stats.created | numberFilter }}</h3>
            <a href="reviewTrades">Review Trades</a>
          </div>
        </b-row>
        <b-row>
          <b-col sm="6" style="padding:0">
            <div class="card-summary-panel">
              <label class="card-label">Trade Exceptions</label>
              <h5 class="trade-count">{{ algo.stats.exceptions | numberFilter }}</h5>
              <a href="reviewExceptions">Review Exceptions</a>
            </div>
          </b-col>
          <b-col sm="6">
            <div class="card-summary-panel">
              <label class="card-label">Order Routed</label>
              <h5 class="trade-count">{{ algo.stats.routed | numberFilter }}</h5>
              <a href="reviewRoutes">Review Routes</a>
            </div>
          </b-col>
        </b-row>
      </b-col>

      <b-col md="8" xs="1">
        <vue-highcharts :options="options" ref="chart"></vue-highcharts>
      </b-col>
    </b-row>

    <div slot="footer">
      <span>{{lastMsg}}</span>
    </div>
  </b-card>
</template>

<script>
import {
  getAlgos,
  enableAlgos,
  deleteAlgoConfig
} from "../../shared/restProvider";
import AlgoConfig from "./AlgoConfig";
import VueHighcharts from "vue2-highcharts";

export default {
  name: "AlgoCard",
  components: {
    AlgoConfig,
    VueHighcharts
  },
  props: ["algo", "parentSize"],
  watch: {
    parentSize: function(newSize, oldSize) {
      console.log(newSize);
      this.$refs.chart.getChart().reflow();
      // console.log(this.$refs.chart.getChart());
    }
  },
  data: function() {
    return {
      timer: null,
      blinkTrades: false,
      lastTick: 0,
      options: {
        chart: {
          type: "spline",
          events: {
            load: this.myLoader
          },
          height: "200"
        },
        title: {
          text: ""
        },
        time: {
          useUTC: false
        },
        xAxis: {
          type: "datetime",
          tickInterval: 1000 * 60
        },
        yAxis: {
          title: {
            text: "Trades"
          }
        },
        plotOptions: {
          series: {
            marker: {
              enabled: false
            }
          }
        },
        tooltip: {
          crosshairs: true,
          shared: true
        },
        credits: {
          enabled: false
        },
        legend: {
          enabled: false
        },
        series: [
          {
            name: "Trades Created",
            data: (() => {
              let data = [];

              this.algo.history.forEach(element => {
                let timeConverted = new Date(element.date).getTime();
                data.push({
                  x: timeConverted,
                  y: element.value,
                  marker: {
                    enabled: false
                  }
                });
              });

              return data;
            })()
          }
        ]
      }
    };
  },
  methods: {
    myLoader(parentObj) {
      // set up the updating of the chart each second
      let series = parentObj.target.series[0];
      this.timer = setInterval(() => {
        let history = this.algo.history;
        let rightNow = new Date().getTime();
        // this logic may not be necessary is highcharts can extend the line to the rightmost boundary
        let lastElement =
          history.length > 0 ? history[history.length - 1] : null;

        if (lastElement) {
          series.addPoint([rightNow, this.algo.stats.created], true, true);
        }
      }, 2000);
    },
    toggleEnabled(algo, shouldEnable) {
      enableAlgos(algo.name, shouldEnable).then(o => {});
    },
    showCode() {
      this.$emit("showCode");
    },
    showParameters() {
      this.$emit("showParameters");
    },
    deleteAlgo() {
      if (confirm("Are you sure you want to delete " + this.algo.name + "?")) {
        deleteAlgoConfig(this.algo.name).then(o => {
          console.log("Deleted: " + this.algo.name);
          clearInterval(this.timer);
        });
      }
    }
  },
  computed: {
    lastMsg: function() {
      return this.algo.lastMsg;
    }
  },
  filters: {
    numberFilter(value) {
      return `${value.toLocaleString()}`;
    }
  }
};
</script>

<style scoped>
.img-active-state {
  margin-left: 6px;
  margin-top: -2px;
}
.green-text {
  color: #4dbd74;
}

.card-enabled-indicator {
  border-right: 1px solid #ccc;
  padding: 8px 0 0 0;
  min-width: 85px;
}

.fa-toggle-on {
  margin: 2px 0;
  font-size: 32px;
  color: #4dbd74;
}
.fa-toggle-off {
  margin: 2px 0;
  font-size: 32px;
  color: gray;
}

a {
  text-decoration: underline;
  color: #20a8d8;
  font-size: 12px;
  margin: 6px 8px 0 0;
}
.dropdown {
  list-style: none;
  color: #20a8d8;
}

.card {
  border-radius: 0;
  border-bottom: none;
}
.card-header {
  padding: 0 6px;
}
.card-header-block {
  padding: 6px;
}
.card-body {
  padding: 10px 0 0 0;
}
.card-summary-panel {
  min-width: 170px;
  margin: 10px 0 10px 0;
}
.card-footer {
  padding: 4px 8px;
  background-color: #222;
  border-bottom: 1px solid #c8ced3;
  border-top: 1px solid #c8ced3;
  color: #c0c0c0;
  min-height: 30px;
}
.card-footer > div {
  overflow-x: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}
.card-label {
  font-weight: bold;
  font-size: 12px;
  margin-bottom: 0;
}

.card-label.large {
  font-size: 16px;
}
.trade-count {
  margin: 0;
}
/* .enable-button-size {    
  min-width: 160px;
} */
</style>