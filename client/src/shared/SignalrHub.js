const signalR = require('@aspnet/signalr');
const env = require("../environment.config.json");

var readyFunction = null;

let connection = new signalR.HubConnectionBuilder()
  .withUrl(env.serverAddress + "/MasterHub")
  .build();

connection.start().then(function () {
  readyFunction();
});

connection.onclose(function() {
  setInterval(() => {
    if (connection.connectionState === 0) {  
      console.log("Connection lost, attempting to reconnect...");
      connection.start().catch(o=> {
        console.error("Connection error");   
      });
    }
  }, 5000); // Restart connection after 5 seconds.
});

export default class SignalrHub {
  constructor(readyFunc) {
    readyFunction = readyFunc;
  }

  connection = connection;

  subscribe(cmd, servicename) {
    connection.invoke("subscribe", cmd, servicename).catch(err => console.error(err));
  }

}