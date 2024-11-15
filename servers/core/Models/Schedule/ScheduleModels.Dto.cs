using System;

namespace Core.Models.Dto;

public partial class NotificationScheduleDto
{
    public NotificationTemplateDto Template { get; set; }

    public DateTime Start { get; set; }
    public TimeSpan Interval { get; set; }
}
