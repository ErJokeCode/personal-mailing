import * as signalR from "@microsoft/signalr";

export const signal = new signalR.HubConnectionBuilder()
    .withUrl("http://193.168.3.39:5000/signal", {
        transport: signalR.HttpTransportType.ServerSentEvents,
        withCredentials: true,
    })
    .build();

signal.start();

