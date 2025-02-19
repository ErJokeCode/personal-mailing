using System.Collections.Generic;
using System.Text.Json;
using Core.Models;
using Core.Routes.Notifications.Dtos;
using Core.Routes.Notifications.DTOs;
using Riok.Mapperly.Abstractions;

namespace Core.Routes.Notifications.Maps;

[Mapper]
public partial class NotificationMapper
{
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