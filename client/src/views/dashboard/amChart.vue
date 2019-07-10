<template>
  <b-card>
    <div slot="header">
      <b-row>
        <b-col sm="5">
          <h4 id="traffic" class="card-title mb-0">{{ algo.name }}</h4>
        </b-col>
        <b-col sm="7" class="d-none d-md-block">
          <b-button-toolbar class="float-right" aria-label="Toolbar with buttons group">
            <i v-if="algo.enabled" class="fa fa-toggle-on" @click="changeEnabled(algo, false)"></i>
            <i v-if="!algo.enabled" class="fa fa-toggle-off" @click="changeEnabled(algo, true)"></i>
          </b-button-toolbar>
        </b-col>
      </b-row>
    </div>
    <div class="hello" ref="chartdiv"></div>
  </b-card>
</template>

<script>
import * as am4core from "@amcharts/amcharts4/core";
import * as am4charts from "@amcharts/amcharts4/charts";
import am4themes_animated from "@amcharts/amcharts4/themes/animated";
import { setInterval } from "timers";
import { last } from "@amcharts/amcharts4/.internal/core/utils/Array";
import { getAlgos, enableAlgos } from "../../shared/algoProvider";

am4core.useTheme(am4themes_animated);

export default {
  name: "HelloWorld",
  props: ["algo"],
  data: function() {
    return {
      chartData: []
    };
  },
  methods: {
    changeEnabled(algo, shouldEnable) {
      enableAlgos(algo.name, shouldEnable).then(o => {});
    }
  },
  watch: {
    history(newValue, oldValue) {
      console.log(" =============history=============== ");
      console.log(newValue);
    },
    algo(newValue, oldValue) {
      console.log(" =============algo=============== ");
      console.log(newValue);
      this.myAlgo = newValue;
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
      var lastElement = (history.length > 0) ? history[history.length-1] : null;
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
.fa-toggle-on {
  font-size: 32px;
  color: #4dbd74;
}
.fa-toggle-off {
  font-size: 32px;
  color: gray;
}
</style>