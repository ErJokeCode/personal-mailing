using System.Collections.Generic;
using Core.Models;
using Core.Routes.Groups.DTOs;
using Notify.Models;
using Notify.Routes.Notifications.DTOs;
using Riok.Mapperly.Abstractions;

namespace Core.Routes.Groups.Maps;

[Mapper]
public partial class GroupMapper
{
    public partial AdminDto Map(Admin group);

    public partial GroupAssignmentDto Map(GroupAssignment group);
    public partial IEnumerable<GroupAssignmentDto> Map(IEnumerable<GroupAssignment> groups);
}
