<template>
  <div class="my-container">
    <ol class="my-breadcrumb">
      <li class="breadcrumb-item" :key="index" v-for="(routeObject, index) in routeRecords">
        <span class="active" v-if="isLast(index)">{{ getName(routeObject) }}</span>
        <router-link :to="routeObject" v-else>{{ getName(routeObject) }}</router-link>
      </li>
    </ol>
    <div class="pin-right">
      <!-- <b-button variant="ghost">
        Open Console
        <i class="fa fa-code" />
      </b-button> -->
      <router-link to="/settings">
        <a>
          <i class="fa fa-cog" style="padding-right: 8px;"></i>Settings
        </a>        
        <!-- <b-button class="top-bar-button" variant="ghost">
          <i class="fa fa-cog" style="padding-right: 8px;"></i>Settings
        </b-button> -->
      </router-link>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    list: {
      type: Array,
      required: true,
      default: () => []
    }
  },
  computed: {
    routeRecords: function() {
      return this.list.filter(route => route.name || route.meta.label);
    }
  },
  methods: {
    getName(item) {
      return item.meta && item.meta.label ? item.meta.label : item.name || null;
    },
    isLast(index) {
      return index === this.list.length - 1;
    }
  }
};
</script>
<style lang="sass">
.my-container 
  background: #555;
  color: white;
  display: flex;

.my-breadcrumb
  max-width: 100%;
  padding: 12px;
  list-style: none;
  display: flex;
  flex: auto 1 1;
  margin: 0;

.pin-right
  padding: 12px 16px;
  a 
    color: white;
  
</style>
