<template class="splitpanel-template">
  <splitpanes watch-slots style="height: 100%" @resized="resized()" @resize="resize()">
    <algo-dash @toggleConsole="toggleConsole" :splitpanes-size="100 - consolesize" ref="dash"></algo-dash>
    <console-aside :splitpanes-size="consolesize"></console-aside>
  </splitpanes>
</template>

<script>
import AlgoDash from "../views/dashboard/AlgoDash";
import ConsoleAside from "../containers/ConsoleAside";
import Splitpanes from "splitpanes";
import "splitpanes/dist/splitpanes.css";
import { setTimeout } from "timers";

export default {
  name: "split-panel",
  components: {
    AlgoDash,
    Splitpanes,
    ConsoleAside
  },
  data() {
    return {
      consolesize: 0
    };
  },
  mounted() {},
  methods: {
    resize() {
      this.$refs.dash.resize();
    },
    resized() {
      this.$refs.dash.resized();
    },
    toggleConsole() {
      if (this.consolesize < 5) {
        this.consolesize = 50;
      } else {
        this.consolesize = 0;
      }
      setTimeout(() => {
        this.resize();
        this.resized();
      }, 250);
    }
  }
};
</script>

<style>
.splitpanel-template {
  height: calc(100vh - 119px);
  padding: 0;
}
.console {
  text-align: end;
}
.splitpanes--vertical > .splitpanes__splitter {
  min-width: 8px;
  cursor: col-resize;
  background: gray;
}
.splitpanes--vertical > .splitpanes__splitter:hover{
  background: #20a8d8;
  transition: background linear 200ms;
}
</style>
