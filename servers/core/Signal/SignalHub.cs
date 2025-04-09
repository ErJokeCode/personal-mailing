using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
}