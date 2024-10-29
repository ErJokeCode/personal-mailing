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
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static class NotificationHandler
{
    public class Message
    {
        public string chat_id { get; set; }
        public string text { get; set; }
    }

    public class DocumentMessage
    {
        public string chat_id { get; set; }
        public IFormFile document { get; set; }
    }

    public static async Task<bool> SendToBot(string chatId, string content)
    {
        HttpClient httpClient = new() { BaseAddress = new Uri("https://api.telegram.org") };

        var message = new Message()
        {
            chat_id = chatId,
            text = content,
        };

        var botToken = Environment.GetEnvironmentVariable("TOKEN_BOT");

        var response = await httpClient.PostAsJsonAsync($"/bot{botToken}/sendMessage", message);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        return response.IsSuccessStatusCode;
    }

    public static async Task<bool> SendDocumentToBot(string chatId, IFormFileCollection documents, string caption)
    {
        HttpClient httpClient = new() { BaseAddress = new Uri("https://api.telegram.org") };

        var content = new MultipartFormDataContent();

        content.Add(new StringContent(chatId), "chat_id");
        content.Add(new StringContent(caption), "caption");

        foreach (var document in documents)
        {
            content.Add(new StreamContent(document.OpenReadStream()), "document", document.FileName);
        }

        var botToken = Environment.GetEnvironmentVariable("TOKEN_BOT");

        var response = await httpClient.PostAsync($"/bot{botToken}/sendDocument", content);

        return response.IsSuccessStatusCode;
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

        foreach (var guid in details.StudentIds)
        {
            var activeStudent = db.ActiveStudents.Find(guid);

            if (activeStudent == null)
            {
                continue;
            }

            bool sent;

            if (documents != null)
            {
                sent = await SendDocumentToBot(activeStudent.ChatId, documents, notification.Content);
            }
            else
            {
                sent = await SendToBot(activeStudent.ChatId, notification.Content);
            }

            if (!sent)
            {
                continue;
            }

            notification.ActiveStudents.Add(activeStudent);
        }

        var fileNames = await DocumentHandler.StoreDocuments(documents);
        notification.FileNames = fileNames;

        db.Notifications.Add(notification);

        var dto = new NotificationDto()
        {
            Id = notification.Id,
            Content = notification.Content,
            Date = notification.Date,
            AdminId = adminId,
            FileNames = notification.FileNames,
        };

        foreach (var active in notification.ActiveStudents)
        {
            dto.StudentIds.Add(active.Id);
        }

        await db.SaveChangesAsync();
        return Results.Ok(dto);
    }

    public static async Task<IResult> GetStudentNotifications(Guid id, CoreDb db)
    {
        var activeStudent = await db.ActiveStudents.Include(a => a.Notifications).SingleAsync(a => a.Id == id);

        if (activeStudent == null)
        {
            return Results.NotFound("Could not find student");
        }

        var notifications = new List<NotificationDto>();

        foreach (var notif in activeStudent.Notifications)
        {
            var dto = new NotificationDto()
            {
                Id = notif.Id,
                Content = notif.Content,
                Date = notif.Date,
                AdminId = notif.AdminId,
                FileNames = notif.FileNames,
            };
            notifications.Add(dto);
        }

        return Results.Ok(notifications);
    }

    public static async Task<IResult> GetAdminNotifications(HttpContext context, UserManager<AdminUser> userManager,
                                                            CoreDb db)
    {
        var id = userManager.GetUserId(context.User);
        var admin = await db.Users.Include(a => a.Notifications)
                        .ThenInclude(n => n.ActiveStudents)
                        .SingleAsync(a => a.Id == id);

        if (admin == null)
        {
            return Results.NotFound("Could not find admin");
        }

        var notifications = new List<NotificationDto>();

        foreach (var notif in admin.Notifications)
        {
            var dto = new NotificationDto()
            {
                Id = notif.Id,
                Content = notif.Content,
                Date = notif.Date,
                AdminId = notif.AdminId,
                FileNames = notif.FileNames,
            };

            foreach (var student in notif.ActiveStudents)
            {
                dto.StudentIds.Add(student.Id);
            }

            notifications.Add(dto);
        }

        return Results.Ok(notifications);
    }
}
