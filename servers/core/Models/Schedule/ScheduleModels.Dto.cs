using System;

namespace Core.Models.Dto;

public partial class NotificationScheduleDto
{
    public NotificationTemplateDto Template { get; set; }
    public AdminUserDto Admin { get; set; }

    public DateTime Start { get; set; }
    public TimeSpan Interval { get; set; }
    public DateTime Next { get; set; }
}
