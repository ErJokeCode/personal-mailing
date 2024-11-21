
using System.Threading.Tasks;
using Core.Utility;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static partial class ScheduleHandler
{
    public static async Task<IResult> DeleteSchedule(int id, CoreDb db)
    {
        var schedule = await db.NotificationSchedules.FindAsync(id);

        if (schedule == null)
        {
            return Results.NotFound("Schedule not found");
        }

        db.NotificationSchedules.Remove(schedule);

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
