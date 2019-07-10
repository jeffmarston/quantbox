
const env = require("../environment.config.json");

export async function getAlgos() {
    const response = await fetch(env.serverAddress + "/api/algo", {
        mode: "cors"
    });
    return await response.json();
}

export async function enableAlgos(algoName, enable) {
    const response = await fetch(
        env.serverAddress + "/api/algo/" + algoName+ "/enabled", {
        method: 'POST', 
        mode: 'cors', 
        headers: {
            'Content-Type': 'application/json',
        },
        body: enable ? "true" : "false"
    });
    return await response.status;
}