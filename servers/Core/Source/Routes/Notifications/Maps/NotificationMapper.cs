
using System.Collections;
using System.Collections.Generic;
using Core.Models;
using Core.Routes.Notifications.Dtos;
using Riok.Mapperly.Abstractions;

namespace Core.Routes.Notifications.Maps;

[Mapper]
public partial class NotificationMapper
{
#pragma warning disable RMG020 // Source member is not mapped to any target member
    public partial NotificationDto Map(Notification notification);
#pragma warning restore RMG020 // Source member is not mapped to any target member

#pragma warning disable RMG020 // Source member is not mapped to any target member
    public partial IEnumerable<NotificationDto> Map(IEnumerable<Notification> notification);
#pragma warning restore RMG020 // Source member is not mapped to any target member
}