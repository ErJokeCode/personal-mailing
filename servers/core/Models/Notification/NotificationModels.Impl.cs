using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Core.Models.Dto;

public partial class NotificationDto : IMappable<NotificationDto, Notification>
{
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

public partial class NotificationStatusDto : IMappable<NotificationStatusDto, NotificationStatus>
{
    public static NotificationStatusDto Map(NotificationStatus orig)
    {
        return new NotificationStatusDto()
        {
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

public static class NotificationExtensions
{
    public static void BuildNotification(this ModelBuilder builder)
    {
        builder.Entity<Notification>()
            .HasMany(e => e.Statuses)
            .WithOne(e => e.Notification)
            .HasForeignKey(e => e.NotificationId);
    }
}
