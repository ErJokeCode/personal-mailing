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

public class NotificationTemplate
{
    public int Id { get; set; }

    public string Content { get; set; }
    public string Date { get; set; }

    public List<int> DocumentIds { get; set; } = [];
    [NotMapped]
    public List<Document> Documents { get; } = [];

    public List<Guid> StudentIds { get; set; } = [];
    [NotMapped]
    public List<ActiveStudent> ActiveStudents { get; } = [];

    public string AdminId { get; set; }
    public AdminUser Admin { get; set; }
}

public static class NotificationTemplateExtensions
{
    public static NotificationTemplate IncludeDocuments(this NotificationTemplate template, CoreDb db)
    {
        template.Documents.AddRange(DocumentHandler.GetFromIds(template.DocumentIds, db));
        return template;
    }

    public static ICollection<NotificationTemplate> IncludeDocuments(this ICollection<NotificationTemplate> templates,
                                                                     CoreDb db)
    {
        foreach (var template in templates)
        {
            template.IncludeDocuments(db);
        }
        return templates;
    }

    public static NotificationTemplate IncludeStudents(this NotificationTemplate template, CoreDb db)
    {
        foreach (var id in template.StudentIds)
        {
            var activeStudent = db.ActiveStudents.Find(id);

            if (activeStudent != null)
            {
                template.ActiveStudents.Add(activeStudent);
            }
        }

        return template;
    }

    public static ICollection<NotificationTemplate> IncludeStudents(this ICollection<NotificationTemplate> templates,
                                                                    CoreDb db)
    {
        foreach (var template in templates)
        {
            template.IncludeStudents(db);
        }

        return templates;
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
