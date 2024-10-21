using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Notify.Models;
using MassTransit;

using Shared.Messages;

namespace Notify;

public static class Handlers
{
    public static IHubContext<SignalHub> Hub = null;

    public class NewStudentAuthedConsumer : IConsumer<NewStudentAuthed>
    {
        public async Task Consume(ConsumeContext<NewStudentAuthed> context)
        {
            await Hub.Clients.All.SendAsync("NewStudentAuthed", context.Message);
        }
    }
}
