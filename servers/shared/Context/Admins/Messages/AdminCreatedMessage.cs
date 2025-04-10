using System;

namespace Shared.Context.Admins.Messages;

public class AdminCreatedMessage : IMessage
{
    public static string TopicName => "admin-created";

    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required DateOnly CreatedAt { get; set; }
}