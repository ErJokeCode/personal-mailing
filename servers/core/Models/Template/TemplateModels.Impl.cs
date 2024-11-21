using System.Collections.Generic;
using System.Linq;
using Core.Handlers;
using Core.Utility;

namespace Core.Models.Dto;

public partial class NotificationTemplateDto : IMappable<NotificationTemplateDto, NotificationTemplate>
{
    public static NotificationTemplateDto Map(NotificationTemplate orig)
    {
        var dto = new NotificationTemplateDto()
        {
            Id = orig.Id,
            Content = orig.Content,
            Date = orig.Date,
            Admin = AdminUserDto.Map(orig.Admin),
        };

        foreach (var student in orig.ActiveStudents)
        {
            dto.Students.Add(ActiveStudentDto.Map(student));
        }

        foreach (var document in orig.Documents)
        {
            dto.Documents.Add(DocumentDto.Map(document));
        }

        return dto;
    }

    public static List<NotificationTemplateDto> Maps(List<NotificationTemplate> origs)
    {
        return origs.Select(o => NotificationTemplateDto.Map(o)).ToList();
    }
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
