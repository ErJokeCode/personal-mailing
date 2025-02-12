
using System.Collections;
using System.Collections.Generic;
using Core.Models;
using Core.Routes.Notifications.Dtos;
using Riok.Mapperly.Abstractions;

namespace Core.Routes.Notifications.Maps;

[Mapper]
public partial class NotificationMapper
{
    public partial NotificationDto Map(Notification notification);
    public partial IEnumerable<NotificationDto> Map(IEnumerable<Notification> notification);
}