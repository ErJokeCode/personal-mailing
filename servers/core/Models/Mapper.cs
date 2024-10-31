using System.Collections.Generic;
using Core.Models.Dto;

namespace Core.Models;

public static class Mapper
{
    public static AdminUserDto Map(AdminUser orig)
    {
        var dto = new AdminUserDto() { Id = orig.Id, Email = orig.Email };

        return dto;
    }

    public static List<AdminUserDto> Map(ICollection<AdminUser> origs)
    {
        var dtos = new List<AdminUserDto>();

        foreach (var orig in origs)
        {
            var dto = Mapper.Map(orig);
            dtos.Add(dto);
        }

        return dtos;
    }

    public static ActiveStudentDto Map(ActiveStudent orig)
    {
        var dto =
            new ActiveStudentDto() { Id = orig.Id, Email = orig.Email, ChatId = orig.ChatId, Info = orig.Student };

        return dto;
    }

    public static List<ActiveStudentDto> Map(ICollection<ActiveStudent> origs)
    {
        var dtos = new List<ActiveStudentDto>();

        foreach (var orig in origs)
        {
            var dto = Mapper.Map(orig);
            dtos.Add(dto);
        }

        return dtos;
    }

    public static NotificationDto Map(Notification orig)
    {
        var dto = new NotificationDto() {
            Id = orig.Id,
            Content = orig.Content,
            Date = orig.Date,
            Admin = Map(orig.Admin),
        };

        foreach (var student in orig.ActiveStudents)
        {
            dto.Students.Add(Map(student));
        }

        foreach (var document in orig.Documents)
        {
            dto.Documents.Add(Mapper.Map(document));
        }

        return dto;
    }

    public static List<NotificationDto> Map(ICollection<Notification> origs)
    {
        var dtos = new List<NotificationDto>();

        foreach (var orig in origs)
        {
            var dto = Mapper.Map(orig);
            dtos.Add(dto);
        }

        return dtos;
    }

    // TODO Probably figure out a smarter way to to this
    // Using generics or something

    public static DocumentDto Map(Document orig)
    {
        var dto = new DocumentDto() {
            Id = orig.Id,
            Name = orig.Name,
            MimeType = orig.MimeType,
            NotificationId = orig.NotificationId,
        };

        return dto;
    }

    public static List<DocumentDto> Map(ICollection<Document> origs)
    {
        var dtos = new List<DocumentDto>();

        foreach (var orig in origs)
        {
            var dto = Mapper.Map(orig);
            dtos.Add(dto);
        }

        return dtos;
    }
}
