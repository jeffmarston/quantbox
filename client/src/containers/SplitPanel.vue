<template class="splitpanel-template">
  <splitpanes style="height: 100%" @resized="resized()" @resize="resize()">
    <algo-dash  splitpanes-size="100" ref="dash"></algo-dash>
    <console-aside splitpanes-size="0"></console-aside>
  </splitpanes>
</template>

<script>
import AlgoDash from "../views/dashboard/AlgoDash";
import ConsoleAside from "../containers/ConsoleAside";
import Splitpanes from "splitpanes";
import "splitpanes/dist/splitpanes.css";

export default {
  name: "split-panel",
  components: {
    AlgoDash,
    Splitpanes,
    ConsoleAside
  },
  data() {
    return {
      dashsize: "large"
    };
  },
  mounted() {},
  methods: {
    resize() {
        this.$refs.dash.resize();
    },
    resized() {
        this.$refs.dash.resized();
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
/* .splitpanes__splitter {
    min-width: 8px;
    cursor: col-resize;
    background: red !important;
} */
</style>
