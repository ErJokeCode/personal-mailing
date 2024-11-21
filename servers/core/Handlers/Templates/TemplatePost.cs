using System;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Core.Handlers.NotificationHandler;

namespace Core.Handlers;

public static partial class TemplateHandler
{
    public static async Task<IResult> SaveNotificationTemplate([FromForm] IFormFileCollection documents,
                                                               [FromForm] string body, CoreDb db, HttpContext context,
                                                               UserManager<AdminUser> userManager)
    {
        var details = JsonSerializer.Deserialize<NotificationDetails>(
            body, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var adminId = userManager.GetUserId(context.User);

        if (adminId == null)
        {
            Results.NotFound("Could not get the admin");
        }

        var notification = new NotificationTemplate()
        {
            Content = details.Content,
            Date = DateTime.Now.ToString(),
            AdminId = adminId,
        };

        var admin = db.Users.Find(adminId);
        notification.Admin = admin;

        foreach (var guid in details.StudentIds)
        {
            var activeStudent = db.ActiveStudents.Find(guid);

            if (activeStudent == null)
            {
                continue;
            }

            notification.ActiveStudents.Add(activeStudent);
        }

        notification.Documents.Clear();
        var docs = await DocumentHandler.StoreDocuments(documents, db);
        foreach (var doc in docs)
        {
            notification.Documents.Add(doc);
        }

        db.NotificationTemplates.Add(notification);
        await db.SaveChangesAsync();

        var dto = NotificationTemplateDto.Map(notification);
        return Results.Ok(dto);
    }
}
