import * as signalR from "@microsoft/signalr";
import { server_url } from "./store";

export const signal = new signalR.HubConnectionBuilder()
    .withUrl(`${server_url}/signal`, {
        transport: signalR.HttpTransportType.ServerSentEvents,
        withCredentials: true,
    })
    .withAutomaticReconnect()
    .build();

signal.start().catch(err => console.log(err));

