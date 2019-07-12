<template>
  <div class="app">
    <AppHeader fixed>
      <!-- <SidebarToggler class="d-lg-none" display="md" mobile />
      <SidebarToggler class="d-md-down-none" display="lg" /> -->

      <h4 style="margin-left: 20px;">Quantraceptive</h4>

      <b-navbar-nav class="ml-auto">
        Open Console
        <AsideToggler class="d-none d-lg-block" :value="false">Open Console</AsideToggler>
      </b-navbar-nav>
    </AppHeader>
    <div class="app-body">

      <AppSidebar fixed>
        <SidebarHeader />
        <SidebarForm />
        <SidebarNav :navItems="nav"></SidebarNav>
        <SidebarFooter />
        <SidebarMinimizer />
      </AppSidebar>

      <main class="main">
        <Breadcrumb :list="list" />
        <div class="container-fluid">
          <router-view :key="$route.fullPath"></router-view>
        </div>
      </main>
      <AppAside fixed  :width="500">
        <!--aside-->
        <ConsoleAside/>
      </AppAside>
    </div>
    <!-- <TheFooter>
      <div>
        <a href="https://ezesoftware.com">{{ statusMsg }}</a>
      </div>
    </TheFooter> -->
  </div>
</template>

<script>
import nav from "@/_nav";
import {
  Header as AppHeader,
  SidebarToggler,
  Sidebar as AppSidebar,
  SidebarFooter,
  SidebarForm,
  SidebarHeader,
  SidebarMinimizer,
  SidebarNav,
  Aside as AppAside,
  AsideToggler,
  Footer as TheFooter,
  Breadcrumb
} from "@coreui/vue";
import DefaultAside from "./DefaultAside";
import DefaultHeaderDropdownAccnt from "./DefaultHeaderDropdownAccnt";
import ConsoleAside from './ConsoleAside'

export default {
  name: "DefaultContainer",
  components: {
    AsideToggler,
    AppHeader,
    AppSidebar,
    AppAside,
    TheFooter,
    Breadcrumb,
    DefaultAside,
    DefaultHeaderDropdownAccnt,
    SidebarForm,
    SidebarFooter,
    SidebarToggler,
    SidebarHeader,
    SidebarNav,
    SidebarMinimizer,
    ConsoleAside
  },
  data() {
    return {
      nav: nav.items,
      statusMsg: "Initializing..."
    };
  },
  mounted() {
    let statuses = ["Ready", "Sending 100 trades", "Establishing connection", "Learning about machines"];
    setInterval(() => {
      let i = Math.floor(Math.random() * statuses.length);  
      this.statusMsg = statuses[i];
    }, 6000);
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
