using System;
using System.Collections.Generic;

namespace Core.Routes.Notifications.DTOs;

public class SendNotificationDto
{
    public required string Content { get; set; }
    public required IEnumerable<Guid> StudentIds { get; set; }
}