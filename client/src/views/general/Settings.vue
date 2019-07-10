<template>
  <div class="animated fadeIn">
      
    <b-row>
      <b-col sm="6">
        <b-card>
          <div slot="header">Servers</div>

          <b-input-group v-for="(svr) in serverData">
            <b-form-input type="text" placeholder="server name" v-model="svr.name"></b-form-input>
            <b-input-group-append>
              <b-button class="btn-ghost-secondary" @click="removeServer(svr)">
                <i class="fa fa-remove"></i>
              </b-button>
            </b-input-group-append>
          </b-input-group>

          <b-form-input
            type="text"
            placeholder="New server"
            v-model="newServer"
            @blur="addServer()"
          ></b-form-input>

          <div slot="footer">
            <b-button type="submit" variant="success" @click="save">Save</b-button>
          </div>
        </b-card>
      </b-col>
      <b-col sm="6">
        <b-card>
          <div slot="header">Database</div>
          <b-form>
            <b-form-group>
              <b-input-group>
                <b-input-group-prepend>
                  <b-input-group-text>
                    <i class="cui-screen-smartphone"></i>
                  </b-input-group-text>
                </b-input-group-prepend>
                <b-form-input type="text" placeholder="Server" v-model="dbSettings.server"></b-form-input>
              </b-input-group>
            </b-form-group>
            <b-form-group>
              <b-input-group>
                <b-input-group-prepend>
                  <b-input-group-text>
                    <i class="fa fa-database"></i>
                  </b-input-group-text>
                </b-input-group-prepend>
                <b-form-input type="text" placeholder="Database" v-model="dbSettings.database"></b-form-input>
              </b-input-group>
            </b-form-group>
            <b-form-group>
              <b-input-group>
                <b-input-group-prepend>
                  <b-input-group-text>
                    <i class="fa fa-user"></i>
                  </b-input-group-text>
                </b-input-group-prepend>
                <b-form-input type="text" placeholder="Username" v-model="dbSettings.username"></b-form-input>
              </b-input-group>
            </b-form-group>
            <b-form-group>
              <b-input-group>
                <b-input-group-prepend>
                  <b-input-group-text>
                    <i class="fa fa-asterisk"></i>
                  </b-input-group-text>
                </b-input-group-prepend>
                <b-form-input type="password" placeholder="Password" v-model="dbSettings.password"></b-form-input>
              </b-input-group>
            </b-form-group>
          </b-form>
          <div slot="footer">
            <b-button type="submit" variant="success" @click="save">Save</b-button>
            <b-button type="submit" @click="clear">Clear</b-button>
          </div>
        </b-card>
      </b-col>
    </b-row>
  </div>
</template>

<script>
const env = require("../../environment.config.json");
const _ = require("lodash");

export default {
  name: "forms",
  data() {
    return {
      dbSettings: {},
      serverData: [],
      environmentData: {},
      newServer: null
    };
  },
  beforeMount() {
    let dbSettingsString = localStorage.getItem("dbSettings");
    if (dbSettingsString) {
      this.dbSettings = JSON.parse(dbSettingsString);
    }
    this.load();
  },
  methods: {
    addServer() {
      if (this.newServer) {
        this.serverData.push({ name: this.newServer });
        this.newServer = null;
      }
    },
    removeServer(server) {
      this.serverData.splice(this.serverData.indexOf(server), 1);
    },
    load() {
      fetch(env.serverAddress + "/api/environment/", {
        mode: "cors"
      }).then(response => {
        if (response.status !== 200) {
          console.error("!! Status Code: " + response.status);
          console.error(response);
          return;
        }
        // Examine the text in the response
        response.json().then(data => {
          this.serverData = data.servers;
        });
      });
    },
    save() {
      localStorage.setItem("dbSettings", JSON.stringify(this.dbSettings));

      fetch(env.serverAddress + "/api/environment/", {
        method: "PUT",
        mode: "cors",
        cache: "no-cache",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          database: [{ name: this.dbSettings.server }],
          servers: this.serverData,
          clients: []
        })
      });
    },
    clear() {
      localStorage.removeItem("dbSettings");
      this.dbSettings = {};
    }
  }
};
</script>