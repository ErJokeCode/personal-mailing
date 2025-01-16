using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MassTransit;

namespace Core.Messages;

public static class StudentConsumer
{
    public class NewActiveStudentConsumer : IConsumer<NewActiveStudent>
    {
        public async Task Consume(ConsumeContext<NewActiveStudent> context)
        {
            var user = BaseConsumer.Hub.Clients.All;
            await user.SendAsync("NewActiveStudent", context.Message);
        }
    }
}
