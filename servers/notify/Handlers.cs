using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Notify.Models;
using NServiceBus;
using Shared.Messages;

namespace Notify;

public static class Handlers
{
    public static IHubContext<SignalHub> Hub = null;

    public class NewStudentAuthHandler : IHandleMessages<NewStudentAuth>
    {
        public async Task Handle(NewStudentAuth message, IMessageHandlerContext context)
        {
            await Hub.Clients.All.SendAsync("NewStudentAuth", message.Student);
        }
    }
}
