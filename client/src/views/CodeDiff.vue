<template>
  <div class="animated component-wrapper">
    <b-row class="toolbar">
      <b-col xs="6">
        <b-form-select
          id="client1Select"
          value="Please select"
          :options="availableClients"
          v-model="profile1.selectedClient"
          @change="updateObjects(profile1)"
        ></b-form-select>
        <b-form-select
          id="object1Select"
          value="Please select"
          :options="profile1.availableObjects"
          v-model="profile1.selectedDbObject"
          @change="loadCode(profile1, 'orig')"
        ></b-form-select>
      </b-col>
      <b-col xs="6">
        <b-form-select
          id="client2Select"
          value="Please select"
          :options="availableClients"
          v-model="profile2.selectedClient"
          @change="updateObjects(profile2)"
        ></b-form-select>
        <b-form-select
          id="object2Select"
          value="Please select"
          :options="profile2.availableObjects"
          v-model="profile2.selectedDbObject"
          @change="loadCode(profile2, 'value')"
        ></b-form-select>
      </b-col>
    </b-row>

    <codemirror :key="cmOptions.orig" :merge="true" ref="myCm" :options="cmOptions"></codemirror>
  </div>
</template>

<script>
// const _ = require("lodash");
// require component and styles
import axios from "axios";
import { codemirror } from "vue-codemirror";
import "codemirror/lib/codemirror.css";
import "codemirror/mode/sql/sql.js";

// language
import "codemirror/mode/css/css.js";
import "codemirror/mode/xml/xml.js";
import "codemirror/mode/htmlmixed/htmlmixed.js";
// merge css
import "codemirror/addon/merge/merge.css";
// merge js
import "codemirror/addon/merge/merge.js";
// google DiffMatchPatch
import DiffMatchPatch from "diff-match-patch";
// DiffMatchPatch config with global
window.diff_match_patch = DiffMatchPatch;
window.DIFF_DELETE = -1;
window.DIFF_INSERT = 1;
window.DIFF_EQUAL = 0;

import { setTimeout } from "timers";

export default {
  name: "code-editor",
  components: { codemirror },
  mounted() {
    axios.get("http://localhost:5000/api/clients").then(response => {
      this.availableClients = response.data;

      console.log(this.$route.query.client);
      console.log(this.$route.query.object);

      if (this.$route.query.client) {
        this.profile1.selectedClient = this.$route.query.client;
        this.profile2.selectedClient = "*2019.5";
      }
      if (this.$route.query.object) {
        this.updateObjects(this.profile1);
        this.updateObjects(this.profile2);
        this.profile1.selectedDbObject = this.$route.query.object;
        this.profile2.selectedDbObject = this.$route.query.object;
        this.loadCode(this.profile1, "value");
        this.loadCode(this.profile2, "orig");
      }
    });
  },
  data() {
    return {
      myCm: {},
      profile1: {
        selectedClient: "",
        selectedDbObject: "",
        availableObjects: []
      },
      profile2: {
        selectedClient: "",
        selectedDbObject: "",
        availableObjects: []
      },
      availableClients: [],
      cmOptions: {
        tabSize: 4,
        mode: "text/x-sql",
        lineNumbers: true,
        line: true,
        value: "",
        orig: "",
        connect: "align",
        lineNumbers: true,
        collapseIdentical: false,
        highlightDifferences: true
      }
    };
  },
  methods: {
    loadCode(profile, target) {
      axios
        .get(
          `http://localhost:5000/api/schema/${profile.selectedClient}/${profile.selectedDbObject}`
        )
        .then(response => {
          if (target === "orig") {
            this.cmOptions.value = response.data;
          } else {
            this.cmOptions.orig = response.data;
          }
          this.$refs.myCm.refresh();
        });
    },
    updateObjects(profile) {
      axios
        .get(
          "http://localhost:5000/api/schema/" +
            profile.selectedClient +
            "/dbobjects"
        )
        .then(response => {
          profile.availableObjects = response.data;
        });
    }
  }
};
</script>

<style lang="scss">
$toolbar-height: 80px;

.component-wrapper {
  height: 100%;
}
.toolbar {
  height: $toolbar-height;
}
.vue-codemirror {
  box-sizing: content-box;
  > div {
    height: 100%;
  }
}
.CodeMirror-merge,
.CodeMirror-merge .CodeMirror {
  height: calc(100vh - #{$toolbar-height} - 120px);
  box-sizing: content-box;
}
</style>
