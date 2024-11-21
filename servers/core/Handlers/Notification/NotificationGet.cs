using System.Threading.Tasks;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class NotificationHandler
{
    public static async Task<IResult> GetNotification(int id, CoreDb db)
    {
        var notification = await db.Notifications.Include(n => n.ActiveStudents)
                               .Include(n => n.Statuses)
                               .Include(n => n.Admin)
                               .Include(n => n.Documents)
                               .SingleOrDefaultAsync(n => n.Id == id);

        if (notification == null)
        {
            return Results.NotFound("Notification not found");
        }

        return Results.Ok(NotificationDto.Map(notification));
    }

    public static async Task<IResult> GetAllNotifications(CoreDb db)
    {
        var notifications = await db.Notifications.Include(n => n.ActiveStudents)
                                .Include(n => n.Admin)
                                .Include(n => n.Statuses)
                                .Include(n => n.Documents)
                                .ToListAsync();

        var dtos = NotificationDto.Maps(notifications);

        return Results.Ok(dtos);
    }
}
