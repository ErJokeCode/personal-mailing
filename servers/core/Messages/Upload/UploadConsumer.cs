using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MassTransit;

namespace Core.Messages;

public static class UploadConsumer
{
    public class UploadDoneConsumer : IConsumer<UploadDone>
    {
        public async Task Consume(ConsumeContext<UploadDone> context)
        {
            var user = BaseConsumer.Hub.Clients.All;
            await user.SendAsync("UploadDone", context.Message);
        }
    }
}
