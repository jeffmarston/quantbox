<template>
    <b-card>
      <div slot="header">
        <b-row>
          <b-col xs="2" class="card-enabled-indicator">
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
            <b-button-toolbar class="float-right" aria-label="Toolbar with buttons group">
              <a @click="toggleConfigMode" >Options...</a>
              <i v-if="algo.enabled" class="fa fa-toggle-on" @click="toggleEnabled(algo, false)"></i>
              <i v-if="!algo.enabled" class="fa fa-toggle-off" @click="toggleEnabled(algo, true)"></i>
            </b-button-toolbar>
          </b-col>
        </b-row>
      </div>

      <b-row  v-if="!configMode">
        <b-col sm="3">
          <div class="card-summary-panel">
            <strong>Trades Created</strong>
            <h3 class="trade-count" :class="{ blinky: newTrades }">{{ algo.tradesCreated }}</h3>
            <a href="reviewTrades">Review Trades</a>
          </div>
          <div class="card-summary-panel">
            <strong>Compliance Violations</strong>
            <h3 class="trade-count">0</h3>
            <a href="reviewCompliance">Review Compliance Alerts</a>
          </div>
        </b-col>
        <b-col sm="9">
          <div class="hello" ref="chartdiv"></div>
        </b-col>
      </b-row>

      <algo-config :algoName="algo.name" v-if="configMode"></algo-config>    

      <div slot="footer">
        <span>{{lastMsg}}</span>
      </div>
    </b-card>

</template>

<script>
import * as am4core from "@amcharts/amcharts4/core";
import * as am4charts from "@amcharts/amcharts4/charts";
import am4themes_animated from "@amcharts/amcharts4/themes/animated";
import { setInterval } from "timers";
import { last } from "@amcharts/amcharts4/.internal/core/utils/Array";
import { getAlgos, enableAlgos } from "../../shared/algoProvider";
import { random } from "@amcharts/amcharts4/.internal/core/utils/String";
import AlgoConfig from "../admin/AlgoConfig";

am4core.useTheme(am4themes_animated);

export default {
  name: "AlgoCard",
  components: {
    AlgoConfig
  },
  props: ["algo"],
  data: function() {
    return {
      chartData: [],
      configMode: false
    };
  },
  methods: {
    toggleEnabled(algo, shouldEnable) {
      enableAlgos(algo.name, shouldEnable).then(o => {});
    },
    toggleConfigMode() {
      
      this.$emit('showOptions', true);
      this.configMode = !this.configMode;
    }
  },
  computed: {
    newTrades: function() {
      if (this.algo.history.length >= 2) {
        let lastElement = this.algo.history[this.algo.history.length - 1];
        let penultimate = this.algo.history[this.algo.history.length - 2];
        if (lastElement.value !== penultimate.value) {
          return true;
        }
      }
      return false;
    },
    lastMsg: function() {
      return this.algo.lastMsg;
    }
  },
  mounted() {
    let chart = am4core.create(this.$refs.chartdiv, am4charts.XYChart);

    chart.paddingRight = 20;

    let data = [];
    const timeWindow = 240; // seconds
    var startTime = new Date(Date.now() - timeWindow * 1000);

    let visits = 10;
    let lastDate = 0;
    for (let i = 1; i < timeWindow; i++) {
      visits += Math.round((Math.random() < 0.5 ? 1 : 0) * Math.random() * 10);
      data.push({
        date: new Date(startTime.setSeconds(startTime.getSeconds() + 1)),
        name: "name" + i,
        value: visits
      });
      lastDate = i;
    }
    chart.data = data;

    // -------------------------------------------------------------

    setInterval(() => {
      var history = this.algo.history;
      var rightNow = new Date().setMilliseconds(0);
      var lastElement = history.length > 0 ? history[history.length - 1] : null;

      if (lastElement && lastElement.date < new Date()) {
        history.push({
          date: rightNow,
          name: "point_" + rightNow,
          value: lastElement.value
        });
      }

      chart.data = this.algo.history;
    }, 1000);

    // -------------------------------------------------------------

    let dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.renderer.grid.template.location = 0;
    dateAxis.baseInterval = {
      timeUnit: "minutes",
      count: 5
    };
    dateAxis.tooltipDateFormat = "HH:mm:ss";

    let valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.tooltip.disabled = true;
    valueAxis.renderer.minWidth = 35;

    let series = chart.series.push(new am4charts.LineSeries());
    series.dataFields.dateX = "date";
    series.dataFields.valueY = "value";

    series.tooltipText = "{valueY.value}";
    chart.cursor = new am4charts.XYCursor();

    // let scrollbarX = new am4charts.XYChartScrollbar();
    // scrollbarX.series.push(series);
    // chart.scrollbarX = scrollbarX;

    this.chart = chart;
  },

  beforeDestroy() {
    if (this.chart) {
      this.chart.dispose();
    }
  }
};
</script>

<style scoped>
.img-active-state {
  margin-left: 6px;
  margin-top: -2px;
}

@keyframes blink {
  0% {
    color: rgba(0, 200, 0, 0.9);
  }
  100% {
    color: rgba(0, 0, 0, 1);
  }
}
.blinky {
  color: rgba(0, 200, 0, 1);
  animation: blink normal 200ms linear;
}

.card-enabled-indicator {
  border-right: 1px solid #ccc;
  padding: 6px 0 0 0;
}

.fa-toggle-on {
  font-size: 32px;
  color: #4dbd74;
}
.fa-toggle-off {
  font-size: 32px;
  color: gray;
}
a {
  text-decoration: underline;
  color: #20a8d8;
  font-size: 12px;
}

.card {
  border-radius: 0;
  border-bottom: none;
}
.card-header {
  padding: 0 6px;
}
.card-header-block {
  padding: 4px 6px;
}
.card-body {
  padding: 10px 0 0 0;
}
.card-summary-panel {
  min-width: 170px;
  margin: 10px 0 10px 0;
}
.card-summary-panel h3 {
  margin: 0;
}
.card-footer {
  padding: 4px 8px;
  background-color: #000000;
  border-bottom: 1px solid #c8ced3;
  border-top: 1px solid #c8ced3;
  color: #20d82f;
  min-height: 30px;
}
.card-footer > div {
  overflow-x: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}
</style>