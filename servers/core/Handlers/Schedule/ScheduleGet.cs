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
}
