using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using MassTransit;
using System.Net.Http.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Core.Messages;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Core;

public static class Handlers
{
    public class AuthDetails
    {
        public string Email { get; set; }

        [JsonPropertyName("personal_number")]
        public string PersonalNumber { get; set; }

        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }
    }

    public static async Task<IResult> AuthStudent(AuthDetails details, IPublishEndpoint endpoint, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.SingleOrDefault(a => a.Email == details.Email);

        if (activeStudent != null)
        {
            await activeStudent.IncludeStudent();
            return Results.Ok(activeStudent);
        }

        activeStudent = new ActiveStudent()
        {
            Email = details.Email,
            ChatId = details.ChatId,
        };

        var found = await activeStudent.IncludeStudent();

        if (!found || activeStudent.Student.PersonalNumber != details.PersonalNumber)
        {
            return Results.NotFound("Could not find the student");
        }

        db.ActiveStudents.Add(activeStudent);
        await db.SaveChangesAsync();

        await endpoint.Publish<NewStudentAuthed>(new()
        {
            ActiveStudent = activeStudent,
        });

        return Results.Created<ActiveStudent>("", activeStudent);
    }

    public static async Task<IResult> GetStudentCourses(Guid id, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.Find(id);

        if (activeStudent == null)
        {
            return Results.NotFound("Could not find the student");
        }

        await activeStudent.IncludeStudent();

        return Results.Ok(activeStudent.Student.OnlineCourse);
    }

    public static async Task<IResult> GetStudents(CoreDb db)
    {
        var activeStudents = db.ActiveStudents.ToArray();
        await activeStudents.IncludeStudents();

        return Results.Ok(activeStudents);
    }

    public class Message
    {
        public string chat_id { get; set; }
        public string text { get; set; }
    }

    public static async Task<bool> SendToBot(string chatId, string content)
    {
        HttpClient httpClient = new() { BaseAddress = new Uri("https://api.telegram.org") };

        var message = new Message()
        {
            chat_id = chatId,
            text = content,
        };

        var bot_token = Environment.GetEnvironmentVariable("TOKEN_BOT");

        var response = await httpClient.PostAsJsonAsync($"/bot{bot_token}/sendMessage", message);

        return response.IsSuccessStatusCode;
    }

    public class NotificationDetails
    {
        public List<Guid> StudentIds { get; set; }

        public string Content { get; set; }
    }

    public static async Task<IResult> SendNotification(NotificationDetails details, CoreDb db, HttpContext context,
                                                       UserManager<AdminUser> userManager)
    {
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

            var sent = await SendToBot(activeStudent.ChatId, notification.Content);

            if (!sent)
            {
                continue;
            }

            notification.ActiveStudents.Add(activeStudent);
        }

        db.Notifications.Add(notification);

        var dto = new NotificationDto()
        {
            Id = notification.Id,
            Content = notification.Content,
            Date = notification.Date,
            AdminId = adminId,
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
            };

            foreach (var student in notif.ActiveStudents)
            {
                dto.StudentIds.Add(student.Id);
            }

            notifications.Add(dto);
        }

        return Results.Ok(notifications);
    }

    public static async Task<T> GetFromParser<T>(string path, Dictionary<string, string> query)
    {
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://parser:8000"),
        };

        var response = await httpClient.GetAsync(QueryHelpers.AddQueryString(path, query));

        if (!response.IsSuccessStatusCode)
        {
            return default(T);
        }

        var serializerSettings = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        var result = await response.Content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<T>(result, serializerSettings);

        return obj;
    }
}

public static class ActiveStudentExtensions
{
    public static async Task<bool> IncludeStudent(this ActiveStudent active)
    {
        var query = new Dictionary<string, string>
        {
            ["email"] = active.Email,
        };

        var student = await Handlers.GetFromParser<Student>("/student", query);

        if (student == null)
        {
            return false;
        }

        active.Student = student;
        return true;
    }

    public static async Task IncludeStudents(this ICollection<ActiveStudent> actives)
    {
        foreach (var active in actives)
        {
            await active.IncludeStudent();
        }
    }
}
