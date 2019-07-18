<template>
  <div class="animated">
    <b-row>
      <b-col xl="12" class="no-pad" >
        <codemirror ref="myCm" :value="code" :options="cmOptions"></codemirror>
      </b-col>
    </b-row>
  </div>
</template>

<script>
// const _ = require("lodash");
// require component and styles
import { codemirror } from "vue-codemirror";
import "codemirror/lib/codemirror.css";
import "codemirror/mode/javascript/javascript.js";
// theme css
import "codemirror/theme/monokai.css";
import "codemirror/theme/base16-dark.css";
import "codemirror/theme/darcula.css";
import "codemirror/theme/twilight.css";
import { setTimeout } from "timers";

export default {
  name: "code-editor",
  components: { codemirror },
  mounted() {
    setTimeout(() => {
      this.$refs.myCm.refresh();
    }, 100);
  },
  data: () => {
    return {
      myCm: {},
      code: `      
import { ezePricing } from "eze-pricing";
import { ezeEvents } from "eze-events";
let pricingFeed = ezePricing.feed;
setInterval(() => {
      data.splice(0, 1);
      if (this.isEnabled) {
        visits += Math.round(
          (Math.random() < 0.5 ? 1 : 0) * Math.random() * 10
        );
      }
      data.push({
        date: new Date(startTime.setSeconds(startTime.getSeconds() + 1)),
        name: "name" + lastDate,
        value: visits
      });
      lastDate++;
      chart.data = data;
    }, 1000);

    setInterval(() => {
      data.splice(0, 1);
      if (this.isEnabled) {
        visits += Math.round(
          (Math.random() < 0.5 ? 1 : 0) * Math.random() * 10
        );
      }
      data.push({
        date: new Date(startTime.setSeconds(startTime.getSeconds() + 1)),
        name: "name" + lastDate,
        value: visits
      });
      lastDate++;
      chart.data = data;
    }, 1000);

    setInterval(() => {
      data.splice(0, 1);
      if (this.isEnabled) {
        visits += Math.round(
          (Math.random() < 0.5 ? 1 : 0) * Math.random() * 10
        );
      }
      data.push({
        date: new Date(startTime.setSeconds(startTime.getSeconds() + 1)),
        name: "name" + lastDate,
        value: visits
      });
      lastDate++;
      chart.data = data;
    }, 1000);
    setInterval(() => {
      data.splice(0, 1);
      if (this.isEnabled) {
        visits += Math.round(
          (Math.random() < 0.5 ? 1 : 0) * Math.random() * 10
        );
      }
      data.push({
        date: new Date(startTime.setSeconds(startTime.getSeconds() + 1)),
        name: "name" + lastDate,
        value: visits
      });
      lastDate++;
      chart.data = data;
    }, 1000);
`,
      cmOptions: {
        tabSize: 4,
        mode: "text/javascript",
        theme: 'darcula',
        //theme: 'twilight',
        //theme: "monokai",
        //theme: "default",
        lineNumbers: true,
        line: true,
        height: 500
      }
    };
  },
  methods: {
    populateAlgo() {}
  }
};
</script>

<style>
.no-pad {
  padding:0;
}
.form-row {
  margin-bottom: 2px;
}
.CodeMirror {
  height: 400px;
}
</style>
