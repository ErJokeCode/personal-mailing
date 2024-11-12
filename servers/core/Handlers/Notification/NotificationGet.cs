using System.Linq;
using System.Threading.Tasks;
using Core.Models;
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
                               .SingleOrDefaultAsync(n => n.Id == id);

        if (notification == null)
        {
            return Results.NotFound("Notification not found");
        }

        notification.IncludeDocuments(db);

        return Results.Ok(NotificationDto.Map(notification));
    }

    public static async Task<IResult> GetAllNotifications(CoreDb db)
    {
        var notifications = await db.Notifications.Include(n => n.ActiveStudents)
                                .Include(n => n.Admin)
                                .Include(n => n.Statuses)
                                .ToListAsync();

        notifications.IncludeDocuments(db);

        var dtos = NotificationDto.Maps(notifications);

        return Results.Ok(dtos);
    }

    public static IResult GetAllTemplates(CoreDb db)
    {
        var templates = db.NotificationTemplates.Include(n => n.Admin).ToList();

        templates.IncludeDocuments(db).IncludeStudents(db);

        return Results.Ok(NotificationTemplateDto.Maps(templates.ToList()));
    }

    public static async Task<IResult> GetTemplate(int id, CoreDb db)
    {
        var template = await db.NotificationTemplates.Include(n => n.Admin).SingleOrDefaultAsync(n => n.Id == id);

        if(template == null)
        {
            return Results.NotFound("Template not found");
        }

        template.IncludeDocuments(db).IncludeStudents(db);

        return Results.Ok(NotificationTemplateDto.Map(template));
    }
}
