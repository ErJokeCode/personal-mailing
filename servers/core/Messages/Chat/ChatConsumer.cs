using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MassTransit;

namespace Core.Messages;

public static class ChatConsumer
{
    public class StudentSentMessageConsumer : IConsumer<StudentSentMessage>
    {
        public async Task Consume(ConsumeContext<StudentSentMessage> context)
        {
            var user = BaseConsumer.Hub.Clients.User(context.Message.Admin.Id);
            await user.SendAsync("StudentSentMessage", context.Message);
        }
    }

    public class ChatReadConsumer : IConsumer<ChatRead>
    {
        public async Task Consume(ConsumeContext<ChatRead> context)
        {
            var user = BaseConsumer.Hub.Clients.User(context.Message.AdminId);
            await user.SendAsync("ChatRead", context.Message);
        }
    }
}
