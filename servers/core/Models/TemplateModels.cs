using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Handlers;
using Core.Utility;

namespace Core.Models;

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
        template.Documents.Clear();
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
        template.ActiveStudents.Clear();

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
