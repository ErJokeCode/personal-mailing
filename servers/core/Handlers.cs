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

namespace Core;

public static class Handlers
{
    private static async Task<T> GetFromParser<T>(string path, Dictionary<string, string> query)
    {
        HttpClient httpClient = new() {
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

    public static async Task<IResult> HandleCourses(Guid id, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.Find(id);

        if (activeStudent == null)
        {
            return Results.NotFound();
        }

        var query = new Dictionary<string, string> {
            ["email"] = activeStudent.Email,
        };

        var student = await GetFromParser<Student>("/student", query);

        if (student == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(student.OnlineCourse);
    }

    public class AuthDetails
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("personal_number")]
        public string PersonalNumber { get; set; }
        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }
    }
    public static async Task<IResult> HandleAuth(AuthDetails details, IPublishEndpoint endpoint, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.SingleOrDefault(a => a.Email == details.Email);

        if (activeStudent != null)
        {
            return Results.Ok(activeStudent);
        }

        var query = new Dictionary<string, string> {
            ["email"] = details.Email,
        };

        var student = await GetFromParser<Student>("/student", query);

        if (student == null || student.PersonalNumber != details.PersonalNumber)
        {
            return Results.NotFound();
        }

        activeStudent = new ActiveStudent() {
            Email = student.Email,
            ChatId = details.ChatId,
        };

        db.ActiveStudents.Add(activeStudent);
        await db.SaveChangesAsync();

        await endpoint.Publish<NewStudentAuthed>(new() {
            Student = activeStudent,
        });

        return Results.Created<ActiveStudent>("", activeStudent);
    }

    // public static IResult HandleStudents(CoreDb db)
    // {
    //     return Results.Ok(db.Students);
    // }

    // public class MessageDetails
    // {
    //     public Guid StudentId { get; set; }
    //     public string Content { get; set; }
    // }

    // public class Data
    // {
    //     public string chat_id { get; set; }
    //     public string text { get; set; }
    // }

    // public static async Task<bool> SendToBot(string chatId, string content)
    // {
    //     HttpClient httpClient = new() { BaseAddress = new Uri("https://api.telegram.org") };
    //
    //     var data = new Data() {
    //         chat_id = chatId,
    //         text = content,
    //     };
    //
    //     var bot_token = Environment.GetEnvironmentVariable("TOKEN_BOT");
    //
    //     Console.WriteLine(bot_token);
    //
    //     var response = await httpClient.PostAsJsonAsync($"/bot{bot_token}/sendMessage", data);
    //
    //     return response.IsSuccessStatusCode;
    // }

    // public static async Task<IResult> SendNotification(MessageDetails details, CoreDb db)
    // {
    //     var student = await db.Students.FindAsync(details.StudentId);
    //
    //     if (student == null)
    //     {
    //         return Results.NotFound();
    //     }
    //
    //     var sent = await SendToBot(student.ChatId, details.Content);
    //
    //     if (!sent)
    //     {
    //         return Results.BadRequest();
    //     }
    //
    //     var notification = new Notification() { StudentId = details.StudentId, Content = details.Content };
    //     db.Notifications.Add(notification);
    //
    //     await db.SaveChangesAsync();
    //
    //     return Results.Ok();
    // }
}
