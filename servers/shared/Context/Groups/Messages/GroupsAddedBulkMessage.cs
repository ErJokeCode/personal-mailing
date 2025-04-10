using System.Collections.Generic;

namespace Shared.Context.Groups.Messages;

public class GroupsAddedBulkMessage : IMessage
{
    public static string TopicName => "groups-added-bulk";

    public required List<GroupAddedMessage> Groups { get; set; }
}