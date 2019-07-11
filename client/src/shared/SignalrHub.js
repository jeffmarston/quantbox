const signalR = require('@aspnet/signalr');
const env = require("../environment.config.json");

var readyFunction = null;

let connection = new signalR.HubConnectionBuilder()
  .withUrl(env.serverAddress + "/MasterHub")
  .build();

connection.start().then(function () {
  readyFunction();
});

export default class SignalrHub {
  constructor(readyFunc) {
    readyFunction = readyFunc;
  }

  connection = connection;

  subscribe(cmd, servicename) {
    connection.invoke("subscribe", cmd, servicename).catch(err => console.error(err));
  }

  getServices() {
    connection.invoke("GetServices").catch(err => console.error(err));
  }

}