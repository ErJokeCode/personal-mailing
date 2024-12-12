import * as signalR from "@microsoft/signalr";

export const signal = new signalR.HubConnectionBuilder()
    .withUrl("http://193.160.209.55:5000/signal", {
        transport: signalR.HttpTransportType.ServerSentEvents,
        withCredentials: true,

    })
    .withAutomaticReconnect()
    .build();

