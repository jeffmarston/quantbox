<template>
  <div class="app">
    <AppHeader fixed>
      <SidebarToggler class="d-lg-none" display="md" mobile />
      <!-- <b-link class="navbar-brand" to="#">
        <img class="navbar-brand-full" src="img/brand/logo.svg" width="170" height="55" alt="Ezesoft Logo">
        <img class="navbar-brand-minimized" src="img/brand/sygnet.svg" width="30" height="30" alt="Ezesoft Logo">
      </b-link>-->
      <SidebarToggler class="d-md-down-none" display="lg" />

      <h4>Quantraceptive</h4>

      <b-navbar-nav class="ml-auto">
        <!-- <b-nav-item class="d-md-down-none" to="/alerts">
          <i class="icon-bell"></i>
          <b-badge pill variant="danger">3</b-badge>
        </b-nav-item> 
        <DefaultHeaderDropdownAccnt/>-->
        <AsideToggler class="d-none d-lg-block" />
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
