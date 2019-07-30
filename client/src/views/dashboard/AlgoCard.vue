<template>
  <b-card>
    <div class="card-header">
      <div class="card-header-enabled">
        <span v-if="algo.enabled">
          <img class="img-active-state" src="img/ActivePulseAnimated.gif" alt="Active" />
          Enabled
        </span>
        <span v-if="!algo.enabled">
          <img class="img-active-state" src="img/ux-i-offline.svg" alt="Active" />
          Disabled
        </span>
      </div>
      <div class="card-header-title">
        <h5 class="card-title mb-0">{{ algo.name }}</h5>
      </div>
      <div class="card-header-toggle">
        <b-button-toolbar class="float-right enable-button-size">
          <b-nav-item-dropdown text="Options" right>
            <b-dropdown-item href="#" @click.prevent="showCode">Code</b-dropdown-item>
            <b-dropdown-item href="#" @click.prevent="showParameters">Parameters</b-dropdown-item>
            <b-dropdown-item href="#" @click.prevent="deleteAlgo">Delete</b-dropdown-item>
          </b-nav-item-dropdown>

          <i v-if="algo.enabled" class="fa fa-toggle-on" @click="toggleEnabled(algo, false)"></i>
          <i v-if="!algo.enabled" class="fa fa-toggle-off" @click="toggleEnabled(algo, true)"></i>
        </b-button-toolbar>
      </div>
    </div>

    <div class="card-content">
      <div class="card-body-summary">
        <div class="side-by-side">
          <div class="card-summary-panel">
            <label class="card-label large">Trades Created</label>
            <h3
              class="counter-format"
              :class="{ 'green-text': algo.enabled }"
            >{{ algo.stats.total | numberFilter }}</h3>
            <a href="reviewTrades">Review Trades</a>
          </div>
          <div class="card-summary-panel">
            <label class="card-label large">Completion</label>
            <h3 class="counter-format">{{ algo.stats.completedPct | numberFilter }} %</h3>
            <a href="reviewCompletions">Review Completion</a>
          </div>
        </div>
        <div class="side-by-side">
          <div class="card-summary-panel">
            <label class="card-label">Trade Exceptions</label>
            <h5 class="counter-format">{{ algo.stats.deleted | numberFilter }}</h5>
            <a href="reviewExceptions">Review Exceptions</a>
          </div>
          <div class="card-summary-panel">
            <label class="card-label">Working Orders</label>
            <h5 class="counter-format">{{ algo.stats.working | numberFilter }}</h5>
            <a href="reviewRoutes">Review Working Orders</a>
          </div>
        </div>

        <div class="side-by-side">
          <div class="card-summary-panel">
            <label class="card-label">Auto Executed</label>
            <h5 class="counter-format">{{ algo.stats.autoRouted | numberFilter }}</h5>
            <a href="reviewExceptions">Review Automated</a>
          </div>
          <div class="card-summary-panel">
            <label class="card-label">Manually Executed</label>
            <h5 class="counter-format">{{ algo.stats.manual | numberFilter }}</h5>
            <a href="reviewRoutes">Review Manual</a>
          </div>
        </div>

        <div class="side-by-side">
          <div class="card-summary-panel">
            <label class="card-label">Target Value</label>
            <h5 class="counter-format green-text">${{ algo.stats.totalValue | numberFilter }}</h5>
          </div>
          <div class="card-summary-panel">
            <label class="card-label">Completed Value</label>
            <h5 class="counter-format green-text">${{ algo.stats.completedValue | numberFilter }}</h5>
          </div>
        </div>
      </div>

      <div class="card-body-chart">
        <vue-highcharts :options="options" ref="chart"></vue-highcharts>
      </div>
    </div>

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
    parentSize: function(newValue, oldValue) {
      this.$refs.chart.getChart().reflow();
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
          height: "300"
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
          },
          {
            name: "Auto Executed",
            data: (() => {
              let data = [];

              // this.algo.history.forEach(element => {
              //   let timeConverted = new Date(element.date).getTime();
              //   data.push({
              //     x: timeConverted,
              //     y: 0,
              //     marker: {
              //       enabled: false
              //     }
              //   });
              // });

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
      let createdSeries = parentObj.target.series[0];
      let autoSeries = parentObj.target.series[1];
        
      this.timer = setInterval(() => {
        // this logic may not be necessary if highcharts can extend the line to the rightmost boundary
        let lastElement =
          this.algo.history.length > 0
            ? this.algo.history[this.algo.history.length - 1]
            : null;

        if (lastElement) {
          let twosecondslater = new Date(lastElement.date).getTime() + 2000;
          this.algo.history.push({
            date: twosecondslater,
            value: this.algo.stats.total
          });
          
          autoSeries.addPoint(
            [twosecondslater, this.algo.stats.autoRouted],
            true,
            true
          );

          createdSeries.addPoint(
            [twosecondslater, this.algo.stats.total],
            true,
            true
          );
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
        });
      }
    }
  },
  beforeDestroy() {
    clearInterval(this.timer);
  },
  computed: {
    lastMsg: function() {
      return this.algo.lastMsg;
    }
  },
  filters: {
    numberFilter(value) {
      if (value) {
        return `${value.toLocaleString()}`;
      }
      return value;
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
  display: flex;
}

.card-header-enabled {
  border-right: 1px solid #ccc;
  padding: 14px 8px 0 0;
  min-width: 85px;
}
.card-header-title {
  padding: 12px;
  flex-grow: 1;
}
.card-header-toggle {
  padding: 6px;
}

.card-body {
  padding: 0;
}
.card-body-summary {
  padding: 0;
}
.card-body-chart {
  padding: 0;
  flex: auto 1 1;
  width: 0;
  overflow: hidden;
}
.card-content {
  display: flex;
}
.side-by-side {
  display: flex;
}

.card-summary-panel {
  min-width: 110px;
  margin: 10px;
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
.counter-format {
  margin: 0;
}
chart {
  display: block;
  width: 100%;
}
/* .enable-button-size {    
  min-width: 160px;
} */
</style>
