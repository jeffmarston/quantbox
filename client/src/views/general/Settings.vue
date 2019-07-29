<template>
  <div class="animated fadeIn master-page">
    <b-radio-group>
      <b-radio v-model="selectedAdapter" value="EMS">Use EMS Adapter</b-radio>
      <b-radio v-model="selectedAdapter" value="CSV">Use CSV Adapter</b-radio>
    </b-radio-group>

    <b-card v-if="selectedAdapter==='CSV'">
      <div slot="header">CSV Configuration</div>
      <b-form>
        <b-form-group label="Deposit" label-for="deposit" :label-cols="3">
          <b-form-input id="deposit" type="text" v-model="emsConfig.deposit"></b-form-input>
        </b-form-group>
      </b-form>

      <div slot="footer">
        <b-button type="submit" variant="success" @click="save">Save</b-button>
        <span class="error-text">{{errorText}}</span>
      </div>
    </b-card>

    <b-card v-if="selectedAdapter==='EMS'">
      <div slot="header">EMS Configuration</div>
      <b-form>
        <b-form-group label="Server" label-for="gateway" :label-cols="3">
          <b-form-input id="gateway" type="text" v-model="emsConfig.gateway"></b-form-input>
        </b-form-group>

        <b-form-group label="Bank" label-for="bank" :label-cols="3">
          <b-form-input id="bank" type="text" v-model="emsConfig.bank"></b-form-input>
        </b-form-group>

        <b-form-group label="Branch" label-for="branch" :label-cols="3">
          <b-form-input id="branch" type="text" v-model="emsConfig.branch"></b-form-input>
        </b-form-group>

        <b-form-group label="Customer" label-for="customer" :label-cols="3">
          <b-form-input id="customer" type="text" v-model="emsConfig.customer"></b-form-input>
        </b-form-group>

        <b-form-group label="Deposit" label-for="deposit" :label-cols="3">
          <b-form-input id="deposit" type="text" v-model="emsConfig.deposit"></b-form-input>
        </b-form-group>
      </b-form>

      <div slot="footer">
        <b-button type="submit" variant="success" @click="save">Save</b-button>
        <span class="error-text">{{errorText}}</span>
      </div>
    </b-card>
  </div>
</template>

<script>
import { getEmsConfig, saveEmsConfig } from "../../shared/restProvider";

export default {
  name: "settings",
  components: {},
  data() {
    return {
      emsConfig: {},
      errorText: null,
      selectedAdapter: 'CSV'
    };
  },
  mounted() {
    this.load();
  },
  methods: {
    load() {
      getEmsConfig().then(o => {
        this.emsConfig = o;
      });
    },
    save() {
      let promise = saveEmsConfig(this.emsConfig);
      promise.then(o => {
        this.$router.push('/');
      });
      promise.catch(e => {
        this.errorText = 'Error saving: ' + e;
      });
    }
  }
};
</script>

<style>
.master-page {
  padding:12px;
}
.error-text {
  color: red;
  padding: 6px 12px;;
}
</style>
