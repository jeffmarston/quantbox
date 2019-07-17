
const env = require("../environment.config.json");

export async function getAlgos() {
    const response = await fetch(env.serverAddress + "/api/algo", {
        mode: "cors"
    });
    return await response.json();
}

export async function getAllAlgoConfig() {
    const response = await fetch(env.serverAddress + "/api/configuration/algos", {
        mode: "cors"
    });
    return await response.json();
}

export async function getAlgoConfig(algoName) {
    const response = await fetch(env.serverAddress + "/api/configuration/algo/" + algoName, {
        mode: "cors"
    });
    return await response.json();
}

export async function deleteAlgoConfig(algoName) {
    var response = await fetch(
        env.serverAddress + "/api/configuration/algo/" + algoName, {
            method: 'DELETE',
            mode: 'cors',
            headers: { 'Content-Type': 'application/json' }
        });
    return await response.status;
}

export async function getEmsConfig() {
    const response = await fetch(env.serverAddress + "/api/configuration/ems", {
        mode: "cors"
    });
    return await response.json();
}

export async function saveEmsConfig(emsConfig) {
    var response = await fetch(
        env.serverAddress + "/api/configuration/ems", {
            method: 'POST',
            mode: 'cors',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(emsConfig)
        });
    return await response.status;
}

export async function saveAlgoConfig(algoConfig) {
    var saveConfig = algoConfig;
    console.log("Save: " + JSON.stringify(algoConfig));    

    var response = await fetch(
        env.serverAddress + "/api/configuration/algo/" + algoConfig.name, {
            method: 'POST',
            mode: 'cors',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(saveConfig)
        });
    return await response.status;
}

export async function enableAlgos(algoName, enable) {
    const response = await fetch(
        env.serverAddress + "/api/algo/" + algoName + "/enabled", {
            method: 'POST',
            mode: 'cors',
            headers: { 'Content-Type': 'application/json' },
            body: enable ? "true" : "false"
        });
    return await response.status;
}