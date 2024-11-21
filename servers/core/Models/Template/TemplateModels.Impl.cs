using System.Collections.Generic;
using System.Linq;

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
