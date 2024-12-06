import * as signalR from "@microsoft/signalr";

export const signal = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/signal", {
        transport: signalR.HttpTransportType.ServerSentEvents,
        withCredentials: true,
    })
    .build();

signal.start();

