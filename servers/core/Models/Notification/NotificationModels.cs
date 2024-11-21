using System;
using System.Collections.Generic;

namespace Core.Models;

public class Notification
{
    public int Id { get; set; }

    public string Content { get; set; }
    public string Date { get; set; }

    public string AdminId { get; set; }
    public AdminUser Admin { get; set; }

    public ICollection<Document> Documents { get; } = [];
    public ICollection<ActiveStudent> ActiveStudents { get; } = [];
    public ICollection<NotificationStatus> Statuses { get; } = [];
}

public class NotificationStatus : BaseStatus
{
    public Guid StudentId { get; set; }

    public int NotificationId { get; set; }
    public Notification Notification { get; set; }
}
