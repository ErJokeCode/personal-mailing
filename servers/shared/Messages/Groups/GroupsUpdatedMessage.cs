using System;
using System.Collections.Generic;

namespace Shared.Messages.Groups;

public class GroupAssignmentDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required Guid AdminId { get; set; }
}

public class GroupsAddedMessage
{
    public static string TopicName => "groups-updated";

    public required List<GroupAssignmentDto> GroupAssignments { get; set; }
}