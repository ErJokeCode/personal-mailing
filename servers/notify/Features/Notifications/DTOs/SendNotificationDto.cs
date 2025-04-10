using System;
using System.Collections.Generic;

namespace Notify.Features.Notifications.DTOs;

public class SendNotificationDto
{
    public required string Content { get; set; }
    public required IEnumerable<Guid> StudentIds { get; set; }
}