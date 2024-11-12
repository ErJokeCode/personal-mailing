using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Handlers;
using Core.Utility;

namespace Core.Models;

public class Notification
{
    public int Id { get; set; }

    public string Content { get; set; }
    public string Date { get; set; }

    public List<int> DocumentIds { get; set; } = [];
    [NotMapped]
    public List<Document> Documents { get; } = [];

    public string AdminId { get; set; }
    public AdminUser Admin { get; set; }

    public ICollection<ActiveStudent> ActiveStudents { get; } = [];
    public ICollection<NotificationStatus> Statuses { get; } = [];
}

public static class NotificationExtensions
{
    public static Notification IncludeDocuments(this Notification notification, CoreDb db)
    {
        notification.Documents.Clear();
        notification.Documents.AddRange(DocumentHandler.GetFromIds(notification.DocumentIds, db));
        return notification;
    }

    public static ICollection<Notification> IncludeDocuments(this ICollection<Notification> notifications, CoreDb db)
    {
        foreach (var notification in notifications)
        {
            notification.IncludeDocuments(db);
        }
        return notifications;
    }
}

public class Document
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string MimeType { get; set; }
    public string InternalName { get; set; }
}

public class NotificationStatus : BaseStatus
{
    public Guid StudentId { get; set; }

    public int NotificationId { get; set; }
    public Notification Notification { get; set; }
}
