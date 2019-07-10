<template>
  <div class="master-div">
    <p v-for="(line, idx) in lines" v-bind:key="idx">{{line}}</p>
  </div>
</template>

<script>
import SignalrHub from "../shared/SignalrHub";
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
          this.lines.splice(0,1);
        }
        this.lineCount++;
        this.lines.push("[" + new Date().toLocaleTimeString() + "] " + msg);
      });
    }
  }
};
</script>
<style scoped>
.master-div {
  background: #333;
  height: 100%;
  padding: 6px;
  color: limegreen;
  overflow: auto;
}
p {
  font-family: "Source Code Pro", monospace;
  margin: -2px;
}
</style>
