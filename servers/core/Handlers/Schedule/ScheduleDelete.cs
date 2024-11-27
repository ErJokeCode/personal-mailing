
using System.Threading.Tasks;
using Core.Identity;
using Core.Models;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Handlers;

public static partial class ScheduleHandler
{
    public static async Task<IResult> DeleteSchedule(int id, CoreDb db, HttpContext context,
                                                     UserManager<AdminUser> userManager)
    {
        var schedule = await db.NotificationSchedules.FindAsync(id);

        if (schedule == null)
        {
            return Results.NotFound("Schedule not found");
        }

        var adminId = userManager.GetUserId(context.User);

        if (schedule.AdminId != adminId && !context.User.HasClaim(Permissions.ManipulateStudents.Claim, ""))
        {
            return Results.BadRequest("Schedule does not belong to you");
        }

        db.NotificationSchedules.Remove(schedule);

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
