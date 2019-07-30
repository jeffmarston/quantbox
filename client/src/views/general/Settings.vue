<template>
  <div class="animated fadeIn master-page">
    <b-row>
      <b-col md="6">
        <b-radio v-model="activeAdapter" value="CSV">Use CSV Adapter</b-radio>
        <b-card :disabled="isDisabled('CSV')">
          <b-form>
            <b-form-group label="Filename" label-for="deposit" :label-cols="3">
              <b-form-input :disabled="isDisabled('CSV')" id="filename" type="text"></b-form-input>
            </b-form-group>
          </b-form>

          <div slot="footer">
            <b-button
              type="submit"
              variant="success"
              @click="save"
              :disabled="isDisabled('CSV')"
            >Save</b-button>
            <span class="error-text">{{errorText}}</span>
          </div>
        </b-card>
      </b-col>

      <b-col md="6">
        <b-radio v-model="activeAdapter" value="EMS">Use EMS Adapter</b-radio>
        <b-card :disabled="isDisabled('EMS')">
          <b-form>
            <b-form-group label="Server" label-for="gateway" :label-cols="3">
              <b-form-input
                :disabled="isDisabled('EMS')"
                id="gateway"
                type="text"
                v-model="emsSettings.gateway"
              ></b-form-input>
            </b-form-group>

            <b-form-group label="Bank" label-for="bank" :label-cols="3">
              <b-form-input
                :disabled="isDisabled('EMS')"
                id="bank"
                type="text"
                v-model="emsSettings.bank"
              ></b-form-input>
            </b-form-group>

            <b-form-group label="Branch" label-for="branch" :label-cols="3">
              <b-form-input
                :disabled="isDisabled('EMS')"
                id="branch"
                type="text"
                v-model="emsSettings.branch"
              ></b-form-input>
            </b-form-group>

            <b-form-group label="Customer" label-for="customer" :label-cols="3">
              <b-form-input
                :disabled="isDisabled('EMS')"
                id="customer"
                type="text"
                v-model="emsSettings.customer"
              ></b-form-input>
            </b-form-group>

            <b-form-group label="Deposit" label-for="deposit" :label-cols="3">
              <b-form-input
                :disabled="isDisabled('EMS')"
                id="deposit"
                type="text"
                v-model="emsSettings.deposit"
              ></b-form-input>
            </b-form-group>
          </b-form>

          <div slot="footer">
            <b-button
              :disabled="isDisabled('EMS')"
              type="submit"
              variant="success"
              @click="save"
            >Save</b-button>
            <span class="error-text">{{errorText}}</span>
          </div>
        </b-card>
      </b-col>
    </b-row>
  </div>
</template>

<script>
import { getConfig, saveConfig } from "../../shared/restProvider";

export default {
  name: "settings",
  data() {
    return {
      emsSettings: {},
      errorText: null,
      activeAdapter: "CSV"
    };
  },
  mounted() {
    this.load();
  },
  methods: {
    isDisabled: function(adapterName) {
      return this.activeAdapter !== adapterName;
    },
    load() {
      getConfig().then(o => {
        this.emsSettings = o.emsSettings;
        this.activeAdapter = o.activeAdapter;
        if (!this.emsSettings) {
          this.emsSettings = {};
        }
      });
    },
    save() {
      console.log(this.activeAdapter);
      let promise = saveConfig({
        activeAdapter: this.activeAdapter,
        emsSettings: this.emsSettings
      });
      promise.then(o => {
        this.$router.push("/");
      });
      promise.catch(e => {
        this.errorText = "Error saving: " + e;
      });
    }
  }
};
</script>

<style>
.master-page {
  padding: 12px;
}
.error-text {
  color: red;
  padding: 6px 12px;
}
.custom-radio {
  padding: 6px 30px;
}
.card[disabled] {
  background: #eee;
}
</style>
