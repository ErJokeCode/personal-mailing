using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MassTransit;
using Core.Utility;

namespace Core.Messages;

public static class ChatConsumer
{
    public static IHubContext<SignalHub> Hub = null;

    public class StudentSentMessageConsumer : IConsumer<StudentSentMessage>
    {
        public async Task Consume(ConsumeContext<StudentSentMessage> context)
        {
            var user = Hub.Clients.User(context.Message.Admin.Id);
            await user.SendAsync("StudentSentMessage", context.Message);
        }
    }
}
