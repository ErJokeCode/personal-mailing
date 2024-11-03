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
    public ICollection<ActiveStudent> ActiveStudents { get; } = [];

    public ICollection<NotificationStatus> Statuses { get; } = [];
    public ICollection<Document> Documents { get; } = [];
}

public class Document
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string MimeType { get; set; }
    public string InternalName { get; set; }

    public int? NotificationId { get; set; }
    public Notification Notification { get; set; }

    public int? MessageId { get; set; }
    public Message Message { get; set; }
}

public class NotificationStatus : BaseStatus
{
    public Guid StudentId { get; set; }

    public int NotificationId { get; set; }
    public Notification Notification { get; set; }
}
