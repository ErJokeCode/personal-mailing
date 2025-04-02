using System.Collections.Generic;
using System.Text.Json;
using Notify.Consumers.Admins;
using Notify.Consumers.Students;
using Notify.Messages.Admins;
using Notify.Messages.Students;
using Notify.Models;
using Notify.Routes.Notifications.DTOs;
using Riok.Mapperly.Abstractions;

namespace Notify.Routes.Notifications.Maps;

[Mapper]
public partial class NotificationMapper
{
    public partial Admin Map(AdminDto adminDto);
    public partial Student Map(StudentDto adminDto);

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