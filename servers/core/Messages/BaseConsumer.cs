using Core.Utility;
using Microsoft.AspNetCore.SignalR;

namespace Core.Messages;

public static class BaseConsumer
{
    public static IHubContext<SignalHub> Hub = null;
}
