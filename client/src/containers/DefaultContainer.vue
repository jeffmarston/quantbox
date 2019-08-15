<template>
  <div class="app">
    <AppHeader fixed>
      <h4 class="app-title">Schema Compare</h4>
    </AppHeader>
    <div class="app-body">
      <main class="main">
        <Breadcrumb :list="list" />
        <div class="container-fluid main-display-port">
          <router-view :key="$route.fullPath"></router-view>
        </div>
      </main>
    </div>
  </div>
</template>

<script>
import nav from "@/_nav";
import {
  Header as AppHeader
} from "@coreui/vue";
import DefaultAside from "./DefaultAside";
import Breadcrumb from "./Breadcrumb";
import ConsoleAside from './ConsoleAside'

export default {
  name: "DefaultContainer",
  components: {
    AppHeader,
    Breadcrumb,
    ConsoleAside
  },
  data() {
    return {
      nav: nav.items,
      statusMsg: "Initializing..."
    };
  },
  computed: {
    name() {
      return this.$route.name;
    },
    list() {
      return this.$route.matched.filter(
        route => route.name || route.meta.label
      );
    }
  }
};
</script>
<style scoped>
.main-display-port {
    height: calc(100vh - 100px);
}
</style>
