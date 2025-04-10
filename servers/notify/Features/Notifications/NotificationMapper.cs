using System.Collections.Generic;
using System.Text.Json;
using Notify.Features.Notifications.DTOs;
using Notify.Models;
using Riok.Mapperly.Abstractions;

namespace Notify.Routes.Notifications.Maps;

[Mapper]
public partial class NotificationMapper
{
    [MapperIgnoreSource(nameof(Notification.AdminId))]
    public partial NotificationDto Map(Notification notification);
    public partial IEnumerable<NotificationDto> Map(IEnumerable<Notification> notification);

    public SendNotificationDto? Map(string body)
    {
        return JsonSerializer.Deserialize<SendNotificationDto>(
            body,
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        );
    }
}