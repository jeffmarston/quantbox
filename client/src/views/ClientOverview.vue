<template>
  <div class="animated component-wrapper">
    <b-form-select
      id="client1Select"
      value="Please select"
      :options="availableClients"
      v-model="selectedClient"
      @change="loadSchema(selectedClient)"
    ></b-form-select>

    <div v-for="(item, idx) in overview" :key="idx">
      <router-link :to="{ path : 'diff', query: { client: selectedClient, object: item.dbObject }}">{{ item.message }}</router-link>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "client-overview",
  components: {},
  data() {
    return {
      availableClients: [],
      overview: [],
      selectedClient: ""
    };
  },
  mounted() {
    axios.get("http://localhost:5000/api/clients").then(response => {
      this.availableClients = response.data;
    });
  },
  methods: {
    loadSchema(selectedClient) {
      axios
        .get(`http://localhost:5000/api/schema/${selectedClient}/overview`)
        .then(response => {
          this.overview = response.data;
        });
    }
  }
};
</script>

<style lang="scss">
.component-wrapper {
  height: 100%;
}
</style>
