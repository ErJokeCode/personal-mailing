using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Routes.Chats.DTOs;
using Core.Routes.Students.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Core.Signal;

[Authorize]
public class SignalHub : Hub
{
}

public static class HubContextExtensions
{
    public static async Task NotifyOfStudentAuth(this IHubContext<SignalHub> hub, StudentDto student)
    {
        await hub.Clients.All.SendAsync("StudentAuthed", student);
    }

    public static async Task NotifyOfFileUpload(this IHubContext<SignalHub> hub)
    {
        await hub.Clients.All.SendAsync("FileUploaded");
    }

    public static async Task NotifyOfMessage(this IHubContext<SignalHub> hub, Guid adminId, MessageDto message)
    {
        await hub.Clients.User(adminId.ToString()).SendAsync("MessageReceived", message);
    }

    public static async Task NotifyOfChatRead(this IHubContext<SignalHub> hub, Guid adminId, ChatDto chat)
    {
        await hub.Clients.User(adminId.ToString()).SendAsync("ChatReadReceived", chat);
    }
}