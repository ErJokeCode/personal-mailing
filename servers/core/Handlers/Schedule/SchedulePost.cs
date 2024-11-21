using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class ScheduleHandler
{
    public class ScheduleDetails
    {
        public int TemplateId { get; set; }
        public DateTime Start { get; set; }
        public TimeSpan Interval { get; set; }
    }

    public static async Task<IResult> AddSchedule(ScheduleDetails details, CoreDb db)
    {
        var template =
            await db.NotificationTemplates.Include(t => t.Admin).SingleOrDefaultAsync(t => t.Id == details.TemplateId);

        if (template == null)
        {
            return Results.NotFound("Template not found");
        }

        var schedule = new NotificationSchedule()
        {
            TemplateId = template.Id,
            Template = template,
            Start = details.Start,
            Interval = details.Interval,
            Next = details.Start,
        };

        db.NotificationSchedules.Add(schedule);

        await db.SaveChangesAsync();

        return Results.Ok(NotificationScheduleDto.Map(schedule));
    }
}
