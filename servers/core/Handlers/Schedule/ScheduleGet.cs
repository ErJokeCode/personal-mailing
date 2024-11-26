using System.Threading.Tasks;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class ScheduleHandler
{
    public static async Task<IResult> GetAllSchedules(CoreDb db)
    {
        var schedules = await db.NotificationSchedules.Include(s => s.Template).ThenInclude(t => t.Admin).ToListAsync();

        return Results.Ok(NotificationScheduleDto.Maps(schedules));
    }

    public static async Task<IResult> GetSchedule(int id, CoreDb db)
    {
        var schedule = await db.NotificationSchedules.Include(s => s.Template)
                           .ThenInclude(t => t.ActiveStudents)
                           .Include(s => s.Template)
                           .ThenInclude(t => t.Documents)
                           .SingleOrDefaultAsync(s => s.Id == id);

        if (schedule == null)
        {
            return Results.NotFound("Schedule not found");
        }

        return Results.Ok(NotificationScheduleDto.Map(schedule));
    }
}
