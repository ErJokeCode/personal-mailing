
using System.Threading.Tasks;
using Core.Identity;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class ScheduleHandler
{
    public static async Task<IResult> ChangeSchedule(int id, ScheduleDetails details, CoreDb db, HttpContext context,
                                                     UserManager<AdminUser> userManager)
    {
        var schedule = await db.NotificationSchedules.SingleOrDefaultAsync(s => s.Id == id);

        if (schedule == null)
        {
            return Results.NotFound("Schedule not found");
        }

        var template = await db.NotificationTemplates.Include(t => t.ActiveStudents)
                           .Include(t => t.Documents)
                           .SingleOrDefaultAsync(t => t.Id == details.TemplateId);

        if (template == null)
        {
            return Results.NotFound("Template not found");
        }

        var adminId = userManager.GetUserId(context.User);

        if (schedule.AdminId != adminId && !context.User.HasClaim(Permissions.ManipulateStudents.Claim, ""))
        {
            return Results.BadRequest("Schedule does not belong to you");
        }

        schedule.Start = details.Start;
        schedule.Interval = details.Interval;
        schedule.TemplateId = details.TemplateId;
        schedule.Template = template;

        return Results.Ok(NotificationScheduleDto.Map(schedule));
    }
}
