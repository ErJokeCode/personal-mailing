using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Core.Handlers;

public static partial class NotificationHandler
{
    public class NotificationDetails
    {
        public List<Guid> StudentIds { get; set; }

        public string Content { get; set; }
    }

    public static async Task<IResult> SendNotification([FromForm] IFormFileCollection documents, [FromForm] string body,
                                                       CoreDb db, HttpContext context,
                                                       UserManager<AdminUser> userManager)
    {
        var details = JsonSerializer.Deserialize<NotificationDetails>(
            body, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var adminId = userManager.GetUserId(context.User);

        if (adminId == null)
        {
            Results.NotFound("Could not get the admin");
        }

        var notification = new Notification()
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

            var status = new NotificationStatus()
            {
                StudentId = guid,
            };
            status.SetLost();

            notification.Statuses.Add(status);

            var filled = await NotificationHandler.FillTemplate(notification.Content, activeStudent);

            var sent = await BotHandler.SendToBot(activeStudent.ChatId, filled, documents);

            if (sent)
            {
                status.SetSent();
            }
        }

        notification.Documents.Clear();
        var docs = await DocumentHandler.StoreDocuments(documents, db);
        foreach (var doc in docs)
        {
            notification.Documents.Add(doc);
        }

        db.Notifications.Add(notification);
        await db.SaveChangesAsync();

        var dto = NotificationDto.Map(notification);
        return Results.Ok(dto);
    }
}
