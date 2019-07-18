<template>
  <div class="animated">
    <b-form>
      <b-form-group label="Name" label-for="algoName" :label-cols="3">
        <b-form-input id="algoName" type="text" v-model="algoConfig.name"></b-form-input>
      </b-form-group>

      <b-form-group
        description="Comma separated list"
        label="Symbols"
        label-for="symbols"
        :label-cols="3"
      >
        <b-form-input id="symbols" type="text" v-model="algoConfig.symbols"></b-form-input>
      </b-form-group>
      <b-form-group label="Ratios of Buys/Shorts" label-for="bsRatio" :label-cols="3">
        <b-form-input id="bsRatio" type="text" v-model="algoConfig.buyShortRatio"></b-form-input>
      </b-form-group>
      <b-form-group label="Frequency" label-for="frequency" :label-cols="3">
        <b-form-input id="frequency" type="text" v-model="algoConfig.frequencySec"></b-form-input>
      </b-form-group>

      <b-form-group label="Min batch size" label-for="minBatch" :label-cols="3">
        <b-form-input id="minBatch" type="text" v-model="algoConfig.minBatchSize"></b-form-input>
      </b-form-group>
      <b-form-group label="Max batch size" label-for="maxBatch" :label-cols="3">
        <b-form-input id="maxBatch" type="text" v-model="algoConfig.maxBatchSize"></b-form-input>
      </b-form-group>
    </b-form>
  </div>
</template>

<script>
import { getAlgoConfig, saveAlgoConfig } from "../../shared/restProvider";
const _ = require("lodash");

export default {
  name: "algo-config",
  props: ["algoName", "options"],
  data() {
    return {
      algoConfig: {}
    };
  },
  mounted() {
    if (!this.options.create) {
      this.load(this.algoName);
    }    
  },
  methods: {
    load(algoName) {
      getAlgoConfig(algoName).then(o => {
        o.symbols = o.symbols.join(",");
        this.algoConfig = o;
      });
    },
    save() {
      let mine = JSON.parse(JSON.stringify(this.algoConfig));
      mine.symbols = mine.symbols.split(",");
      saveAlgoConfig(mine).then(o => { });
    }
  }
};
</script>

<style scoped>
.col-form-label {
  text-align: end;
}
</style>
