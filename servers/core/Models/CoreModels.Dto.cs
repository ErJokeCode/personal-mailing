using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models.Dto;

public interface IMappable<TDto, TOrig>
{
    public static abstract TDto Map(TOrig orig);
    public static abstract List<TDto> Maps(List<TOrig> origs);
}

public class AdminUserDto : IMappable<AdminUserDto, AdminUser>
{
    public string Id { get; set; }
    public string Email { get; set; }

    public static AdminUserDto Map(AdminUser orig)
    {
        return new AdminUserDto() { Id = orig.Id, Email = orig.Email };
    }

    public static List<AdminUserDto> Maps(List<AdminUser> origs)
    {
        return origs.Select(o => AdminUserDto.Map(o)).ToList();
    }
}

public class ActiveStudentDto : IMappable<ActiveStudentDto, ActiveStudent>
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string ChatId { get; set; }

    public Student Info { get; set; }

    public static ActiveStudentDto Map(ActiveStudent orig)
    {
        return new ActiveStudentDto() { Id = orig.Id, Email = orig.Email, ChatId = orig.ChatId, Info = orig.Student };
    }

    public static List<ActiveStudentDto> Maps(List<ActiveStudent> origs)
    {
        return origs.Select(o => ActiveStudentDto.Map(o)).ToList();
    }
}

public class NotificationDto : IMappable<NotificationDto, Notification>
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }

    public List<DocumentDto> Documents { get; } = [];

    public AdminUserDto Admin { get; set; }
    public List<ActiveStudentDto> Students { get; } = [];

    public static NotificationDto Map(Notification orig)
    {
        var dto = new NotificationDto()
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

    public static List<NotificationDto> Maps(List<Notification> origs)
    {
        return origs.Select(o => NotificationDto.Map(o)).ToList();
    }
}

public class DocumentDto : IMappable<DocumentDto, Document>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string MimeType { get; set; }

    public int NotificationId { get; set; }

    public static DocumentDto Map(Document orig)
    {
        return new DocumentDto()
        {
            Id = orig.Id,
            Name = orig.Name,
            MimeType = orig.MimeType,
            NotificationId = orig.NotificationId,
        };
    }

    public static List<DocumentDto> Maps(List<Document> origs)
    {
        return origs.Select(o => DocumentDto.Map(o)).ToList();
    }
}
