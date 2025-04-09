using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Chatter.Routes.Chats.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Chatter.Routes.Notifications.DTOs;

namespace Chatter.Signal;

public class HttpContextUserIdProvider : IUserIdProvider
{
    public virtual string GetUserId(HubConnectionContext connection)
    {
        var userInfo = connection.GetHttpContext()?.Request.Headers["x-user-info"].ToString();

        if (userInfo is null)
        {
            return "";
        }

        var admin = JsonSerializer.Deserialize<AdminDto>(userInfo);

        if (admin is null)
        {
            return "";
        }

        return admin.Id.ToString();
    }
}

public class SignalHub : Hub
{
}

public static class HubContextExtensions
{
    public static async Task NotifyOfMessage(this IHubContext<SignalHub> hub, Guid adminId, MessageDto message)
    {
        await hub.Clients.User(adminId.ToString()).SendAsync("MessageReceived", message);
    }

    public static async Task NotifyOfChatRead(this IHubContext<SignalHub> hub, Guid adminId, ChatDto chat)
    {
        await hub.Clients.User(adminId.ToString()).SendAsync("ChatReadReceived", chat);
    }
}