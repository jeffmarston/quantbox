faux quant
==============

faux quant is a simulator to demonstrate black box trading integration with the OMS and EMS products.
It uses Vue, signalR, and dotnet core to provide a realtime view of your algos' performance.

## Prerequisites
- node.js [https://nodejs.org](https://nodejs.org)
- dotnet core [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

# Run it!

## Client
```
> cd client
> npm install
> npm build

## Server

The server only needs dependencies installed, so change to the server folder, install dependencies, and run the following:
```
> cd server
> dotnet restore
> dotnet run --console
```

You're up and running with the defaults. Navigate to ```http://localhost:5000``` to start using it.
To view the API documentation, navigate to ```http://localhost:5000/swagger```
