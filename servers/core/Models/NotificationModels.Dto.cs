using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models.Dto;

public class NotificationDto : IMappable<NotificationDto, Notification>
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }

    public List<DocumentDto> Documents { get; } = [];
    public List<NotificationStatusDto> Statuses { get; } = [];

    public AdminUserDto Admin { get; set; }
    public List<ActiveStudentDto> Students { get; } = [];

    public static NotificationDto Map(Notification orig)
    {
        var dto = new NotificationDto() {
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

        foreach (var status in orig.Statuses)
        {
            dto.Statuses.Add(NotificationStatusDto.Map(status));
        }

        return dto;
    }

    public static List<NotificationDto> Maps(List<Notification> origs)
    {
        return origs.Select(o => NotificationDto.Map(o)).ToList();
    }
}

public class NotificationTemplateDto : IMappable<NotificationTemplateDto, NotificationTemplate>
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }

    public AdminUserDto Admin { get; set; }

    public List<DocumentDto> Documents { get; } = [];
    // TODO get that from orig.StudentIds, same as documents
    public List<ActiveStudentDto> Students { get; } = [];

    public static NotificationTemplateDto Map(NotificationTemplate orig)
    {
        var dto = new NotificationTemplateDto() {
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

public class DocumentDto : IMappable<DocumentDto, Document>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string MimeType { get; set; }

    public static DocumentDto Map(Document orig)
    {
        return new DocumentDto() {
            Id = orig.Id,
            Name = orig.Name,
            MimeType = orig.MimeType,
        };
    }

    public static List<DocumentDto> Maps(List<Document> origs)
    {
        return origs.Select(o => DocumentDto.Map(o)).ToList();
    }
}

public class NotificationStatusDto : IMappable<NotificationStatusDto, NotificationStatus>
{
    public int Code { get; set; }
    public string Short { get; set; }
    public string Description { get; set; }
    public Guid StudentId { get; set; }

    public static NotificationStatusDto Map(NotificationStatus orig)
    {
        return new NotificationStatusDto() {
            Code = orig.Code,
            Short = orig.Short,
            Description = orig.Description,
            StudentId = orig.StudentId,
        };
    }

    public static List<NotificationStatusDto> Maps(List<NotificationStatus> origs)
    {
        return origs.Select(o => NotificationStatusDto.Map(o)).ToList();
    }
}
