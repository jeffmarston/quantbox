<template>
  <div class="animated fadeIn">
    <b-row>
      <b-col xs="12">
        <b-card>
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
          </div>
        </b-card>
      </b-col>
    </b-row>
    <b-row>
      <b-col sm="6" v-for="(config, idx) in allAlgoConfig" :key="idx">
        <b-card>
          <algo-config :algoName="config.name"></algo-config>
        </b-card>
      </b-col>
    </b-row>
  </div>
</template>

<script>
import { getAllAlgoConfig, getEmsConfig, saveEmsConfig } from "../../shared/restProvider";
import AlgoConfig from "../admin/AlgoConfig";
const _ = require("lodash");

export default {
  name: "settings",
  components: {
    AlgoConfig
  },
  data() {
    return {
      allAlgoConfig: [],
      emsConfig: {}
    };
  },
  mounted() {
    this.load();
  },
  methods: {
    load() {
      // getAllAlgoConfig().then(o => {
      //   this.allAlgoConfig = o;
      // });

      getEmsConfig().then(o => {
        this.emsConfig = o;
      });
    },
    save() {
      saveEmsConfig(this.emsConfig).then(o => {
      });
    }
  }
};
</script>