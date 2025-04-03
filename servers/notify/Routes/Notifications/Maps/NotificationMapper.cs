using System.Collections.Generic;
using System.Text.Json;
using Notify.Models;
using Notify.Routes.Notifications.DTOs;
using Riok.Mapperly.Abstractions;

namespace Notify.Routes.Notifications.Maps;

[Mapper]
public partial class NotificationMapper
{
    public partial Admin Map(Shared.Messages.Admins.AdminDto adminDto);
    public partial Student Map(Shared.Messages.Students.StudentDto adminDto);

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