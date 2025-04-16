using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Core.Signal;

[Authorize]
public class SignalHub : Hub
{
}

public static class HubContextExtensions
{
}