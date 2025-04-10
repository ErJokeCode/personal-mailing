using System;

namespace Shared.Context.Groups.Messages;

public class GroupAddedMessage : IMessage
{
    public static string TopicName => "group-added";

    public int Id { get; set; }
    public required string Name { get; set; }
    public required Guid AdminId { get; set; }
}