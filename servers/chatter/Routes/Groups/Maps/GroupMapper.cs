using System.Collections.Generic;
using Chatter.Models;
using Chatter.Routes.Groups.DTOs;
using Chatter.Models;
using Chatter.Routes.Notifications.DTOs;
using Riok.Mapperly.Abstractions;

namespace Chatter.Routes.Groups.Maps;

[Mapper]
public partial class GroupMapper
{
    public partial AdminDto Map(Admin group);

    [MapperIgnoreSource(nameof(GroupAssignment.AdminId))]
    public partial GroupAssignmentDto Map(GroupAssignment group);
    public partial IEnumerable<GroupAssignmentDto> Map(IEnumerable<GroupAssignment> groups);
}
