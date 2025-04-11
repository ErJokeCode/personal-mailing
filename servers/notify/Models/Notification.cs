using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Notify.Models;

public enum NotificationState
{
    Lost,
    Sent,
    Read
}

[Owned]
public class NotificationStatus
{
    public Guid StudentId { get; set; }
    public required NotificationState State { get; set; }
    public required string Message { get; set; }
}

public class Notification
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }

    public ICollection<Document> Documents { get; set; } = [];

    public required Guid AdminId { get; set; }
    public Admin? Admin { get; set; }
    public ICollection<Student> Students { get; set; } = [];

    public ICollection<NotificationStatus> Statuses { get; set; } = [];
}