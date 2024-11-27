using System;

namespace Core.Models;

public class NotificationSchedule
{
    public int Id { get; set; }

    public string AdminId { get; set; }
    public AdminUser Admin { get; set; }

    public int TemplateId { get; set; }
    public NotificationTemplate Template { get; set; }

    public DateTime Start { get; set; }
    public TimeSpan Interval { get; set; }
    public DateTime Next { get; set; }
}
