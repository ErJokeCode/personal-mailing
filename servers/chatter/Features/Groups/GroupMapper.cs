using System.Collections.Generic;
using Chatter.Models;
using Chatter.Routes.Groups.DTOs;
using Riok.Mapperly.Abstractions;
using Shared.Context.Groups.Messages;

namespace Chatter.Features.Groups;

[Mapper]
public partial class GroupMapper
{
    [MapperIgnoreTarget(nameof(GroupAssignment.Admin))]
    public partial GroupAssignment Map(GroupAddedMessage groupAddedMessage);

    [MapperIgnoreSource(nameof(GroupAssignment.AdminId))]
    public partial GroupAssignmentDto Map(GroupAssignment group);
    public partial IEnumerable<GroupAssignmentDto> Map(IEnumerable<GroupAssignment> groups);
}
