<template>
  <div class="animated fadeIn">
    <b-row>
      <b-col xs="12">
        <b-card>
          <div slot="header">EMS Configuration</div>

          <b-form>
            <b-form-group label="Server" label-for="emsServer" :label-cols="3">
              <b-form-input id="emsServer" type="text"></b-form-input>
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
import { getAllAlgoConfig } from "../../shared/algoProvider";
import AlgoConfig from "../admin/AlgoConfig";
const _ = require("lodash");

export default {
  name: "settings",
  components: {
    AlgoConfig
  },
  data() {
    return {
      allAlgoConfig: []
    };
  },
  mounted() {
    this.load();
  },
  methods: {
    load() {
      getAllAlgoConfig().then(o => {
        this.allAlgoConfig = o;
      });
    },
    save() {
      //
    }
  }
};
</script>