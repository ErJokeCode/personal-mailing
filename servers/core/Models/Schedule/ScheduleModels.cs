using System;

namespace Core.Models;

public class NotificationSchedule
{
    public int Id { get; set; }

    public int TemplateId { get; set; }
    public NotificationTemplate Template { get; set; }

    public DateTime Start { get; set; }
    public TimeSpan Interval { get; set; }
}
