<template>
  <div class="console-panel">
    <p class="console-line" v-for="(line, idx) in lines" v-bind:key="idx">{{line}}</p>
  </div>
</template>

<script>
import SignalrHub from "../shared/SignalrHub";
let signalrHub = null;
let onReady = function() {};
signalrHub = new SignalrHub(onReady);
let conn = signalrHub.connection;

export default {
  name: "console-aside",
  components: {},
  mounted() {
    this.subscribeToConsole();
  },
  data() {
    return {
      lines: [],
      lineCount: 0
    };
  },
  methods: {
    subscribeToConsole(algos) {
      conn.on("console", msg => {
        if (this.lines.length > 100) {
          this.lines.splice(0, 1);
        }
        this.lineCount++;

        let date = new Date();
        let dateString =
          date.getHours() +
          ":" +
          date.getMinutes() +
          ":" +
          date.getSeconds() +
          "." +
          date.getMilliseconds();
        this.lines.push("[" + dateString + "] " + msg);
      });
    }
  }
};
</script>
<style scoped>
.console-panel {
  background: #333;
  height: 100%;
  padding: 6px;
  overflow: auto;
  white-space: nowrap;
}
.console-line {
  font-family: "Source Code Pro", monospace;
  margin: -2px;
  color: #c0c0c0;
  overflow: hidden;
}
</style>
