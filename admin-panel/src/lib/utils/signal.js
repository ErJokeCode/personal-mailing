import * as signalR from "@microsoft/signalr";
import { ServerUrl } from "../server";

export const signal = new signalR.HubConnectionBuilder()
    .withUrl(`${ServerUrl}/chat-hub`, {
        transport: signalR.HttpTransportType.ServerSentEvents,
        withCredentials: true,
    })
    .withAutomaticReconnect()
    .build();

signal.start().catch(err => console.log(err));

