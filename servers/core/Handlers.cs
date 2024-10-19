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
using NServiceBus;
using Shared.Core;
using Shared.Messages;

namespace Core;

public static class Handlers
{
    private static async Task<T> GetFromParser<T>(string path, Dictionary<string, string> query)
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

        var serializerSettings = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        var result = await response.Content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<T>(result, serializerSettings);

        return obj;
    }

    public static async Task<IResult> HandleCourses(Guid id, CoreDb db)
    {
        var user = db.Students.Find(id);

        if (user == null)
        {
            return Results.NotFound();
        }

        var query = new Dictionary<string, string>
        {
            ["id"] = user.UserCourseId,
        };

        var userCourse = await GetFromParser<UserCourse>("/course/by_id/", query);

        if (userCourse == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(userCourse.Courses);
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

    public static async Task<IResult> HandleAuth(AuthDetails details, IEndpointInstance endpoint, CoreDb db)
    {
        var student = db.Students.SingleOrDefault(s => s.Email == details.Email);

        if (student != null)
        {
            return Results.Ok(student);
        }

        var query = new Dictionary<string, string>
        {
            ["email"] = details.Email,
        };

        var userCourse = await GetFromParser<UserCourse>("/course/by_email/", query);

        if (userCourse == null)
        {
            return Results.NotFound();
        }

        query = new Dictionary<string, string>
        {
            ["name"] = userCourse.Name,
            ["sername"] = userCourse.Sername,
            ["patronymic"] = userCourse.Patronymic,
        };

        var user = await GetFromParser<User>("/user_info/by_name/", query);

        if (user == null || details.PersonalNumber != user.PersonalNumber)
        {
            return Results.NotFound();
        }

        student = new Student()
        {
            Email = details.Email,
            PersonalNumber = details.PersonalNumber,
            ChatId = details.ChatId,
            UserId = user._id,
            UserCourseId = userCourse._id
        };

        db.Students.Add(student);
        await db.SaveChangesAsync();

        await endpoint.Send(new NewStudentAuth() { Student = student });

        return Results.Created<Student>("", student);
    }
}
