using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MassTransit;
using Core.Messages;
using Core.Models;

namespace Core;

public static class MessageHandlers
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
