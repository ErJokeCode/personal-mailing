using System.Collections.Generic;
using System.Linq;

namespace Core.Models.Dto;

public partial class NotificationScheduleDto : IMappable<NotificationScheduleDto, NotificationSchedule>
{
    public static NotificationScheduleDto Map(NotificationSchedule orig)
    {
        return new NotificationScheduleDto()
        {

            Template = NotificationTemplateDto.Map(orig.Template),
            Start = orig.Start,
            Interval = orig.Interval,
        };
    }

    public static List<NotificationScheduleDto> Maps(List<NotificationSchedule> origs)
    {
        return origs.Select(o => NotificationScheduleDto.Map(o)).ToList();
    }
}
