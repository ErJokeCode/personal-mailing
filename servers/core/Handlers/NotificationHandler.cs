using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static class NotificationHandler
{
    public class BotMessage
    {
        public string chat_id { get; set; }
        public string text { get; set; }
    }

    public static async Task<bool> SendToBot(string chatId, string text, IFormFileCollection documents)
    {
        HttpClient httpClient = new() { BaseAddress = new Uri("http://bot:8000/") };

        if (documents.Count > 0)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(chatId), "chat_ids");

            foreach (var document in documents)
            {
                content.Add(new StreamContent(document.OpenReadStream()), "files", document.FileName);
            }

            var uri = QueryHelpers.AddQueryString("/send/files", "text", text);
            var response = await httpClient.PostAsync(uri, content);

            return response.IsSuccessStatusCode;
        }
        else
        {
            var message = new BotMessage()
            {
                chat_id = chatId,
                text = text,
            };

            var uri = QueryHelpers.AddQueryString($"/send/{chatId}", "text", text);
            var response = await httpClient.PostAsJsonAsync(uri, message);

            return response.IsSuccessStatusCode;
        }
    }

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
            notification.ActiveStudents.Add(activeStudent);

            var status = new NotificationStatus()
            {
                StudentId = guid,
            };
            status.SetLost();

            notification.Statuses.Add(status);

            if (activeStudent == null)
            {
                continue;
            }

            var sent = await SendToBot(activeStudent.ChatId, notification.Content, documents);
        }

        db.Notifications.Add(notification);
        await db.SaveChangesAsync();

        await DocumentHandler.StoreDocuments(documents, notification.Id, db);

        var dto = NotificationDto.Map(notification);
        return Results.Ok(dto);
    }

    public static async Task<IResult> SetNotificationStatus(int id, Guid studentId, int code, CoreDb db)
    {
        var status =
            await db.NotificationStatuses.SingleOrDefaultAsync(n => n.NotificationId == id && n.StudentId == studentId);

        if (status == null)
        {
            return Results.NotFound("Status not found");
        }

        switch (code)
        {
            case -1:
                status.SetLost();
                break;
            case 0:
                status.SetSent();
                break;
            case 1:
                status.SetRead();
                break;
            default:
                return Results.BadRequest("Wrong status code");
        }

        await db.SaveChangesAsync();

        return Results.Ok(NotificationStatusDto.Map(status));
    }

    public static async Task<IResult> GetAllNotifications(CoreDb db)
    {
        var notifications = await db.Notifications.Include(n => n.ActiveStudents)
                                .Include(n => n.Documents)
                                .Include(n => n.Admin)
                                .Include(n => n.Statuses)
                                .ToListAsync();

        var dtos = NotificationDto.Maps(notifications);

        return Results.Ok(dtos);
    }

    public static async Task<IResult> GetStudentNotifications(Guid id, CoreDb db)
    {
        var activeStudent = await db.ActiveStudents.Include(a => a.Notifications)
                                .ThenInclude(n => n.Documents)
                                .Include(a => a.Notifications)
                                .ThenInclude(n => n.Admin)
                                .Include(a => a.Notifications)
                                .ThenInclude(n => n.Statuses)
                                .SingleOrDefaultAsync(a => a.Id == id);

        if (activeStudent == null)
        {
            return Results.NotFound("Could not find student");
        }

        var dtos = NotificationDto.Maps((List<Notification>)activeStudent.Notifications);

        return Results.Ok(dtos);
    }

    public static async Task<IResult> GetAdminNotifications(HttpContext context, UserManager<AdminUser> userManager,
                                                            CoreDb db)
    {
        var id = userManager.GetUserId(context.User);
        var admin = await db.Users.Include(a => a.Notifications)
                        .ThenInclude(n => n.ActiveStudents)
                        .Include(a => a.Notifications)
                        .ThenInclude(n => n.Documents)
                        .Include(a => a.Notifications)
                        .ThenInclude(n => n.Statuses)
                        .SingleOrDefaultAsync(a => a.Id == id);

        if (admin == null)
        {
            return Results.NotFound("Could not find admin");
        }

        var dtos = NotificationDto.Maps((List<Notification>)admin.Notifications);

        return Results.Ok(dtos);
    }

    public static async Task<IResult> GetNotification(int id, CoreDb db)
    {
        var notification = await db.Notifications.Include(n => n.ActiveStudents)
                               .Include(n => n.Documents)
                               .Include(n => n.Statuses)
                               .Include(n => n.Admin)
                               .SingleOrDefaultAsync(n => n.Id == id);

        if (notification == null)
        {
            return Results.NotFound("Notification not found");
        }

        return Results.Ok(NotificationDto.Map(notification));
    }
}
