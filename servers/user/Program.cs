using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace User;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connection = builder.Configuration.GetConnectionString("Database");
        builder.Services.AddDbContext<Models.UserDb>(options => options.UseNpgsql(connection));

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<Models.UserDb>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }

        app.MapPost("/auth", HandleAuth);
        app.MapGet("/{id}/courses", HandleCourses);

        app.Run();
    }

    private static async Task<IResult> HandleCourses(Guid id, Models.UserDb db)
    {
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://parser:8000"),
        };

        var user = db.Students.Find(id);

        if (user == null)
        {
            Console.WriteLine("Didnt find user");
            return Results.NotFound();
        }

        var query = new Dictionary<string, string>
        {
            ["id"] = user.UserCourseId,
        };
        var response = await httpClient.GetAsync(QueryHelpers.AddQueryString("/course/by_id/", query));

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Didnt find user's courses");
            return Results.NotFound();
        }

        var serializerSettings = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        var result = await response.Content.ReadAsStringAsync();
        var userCourse = JsonSerializer.Deserialize<Models.UserCourse>(result, serializerSettings);

        return Results.Ok(userCourse.Courses);
    }

    private static async Task<IResult> HandleAuth(Models.AuthDetails details, Models.UserDb db)
    {
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://parser:8000"),
        };
        var query = new Dictionary<string, string>
        {
            ["email"] = details.Email,
        };
        var response = await httpClient.GetAsync(QueryHelpers.AddQueryString("/course/by_email/", query));

        if (!response.IsSuccessStatusCode)
        {
            return Results.NotFound();
        }

        var serializerSettings = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        var result = await response.Content.ReadAsStringAsync();
        var userCourse = JsonSerializer.Deserialize<Models.UserCourse>(result, serializerSettings);

        query = new Dictionary<string, string>
        {
            ["name"] = userCourse.Name,
            ["sername"] = userCourse.Sername,
            ["patronymic"] = userCourse.Patronymic,
        };
        response = await httpClient.GetAsync(QueryHelpers.AddQueryString("/user_info/by_name/", query));

        if (!response.IsSuccessStatusCode)
        {
            return Results.NotFound();
        }

        result = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<Models.User>(result, serializerSettings);

        if (details.PersonalNumber != user.PersonalNumber)
        {
            return Results.NotFound();
        }

        var student =
            new Models.Student()
            {
                Email = details.Email,
                PersonalNumber = details.PersonalNumber,
                ChatId = details.ChatId,
                UserId = user._id,
                UserCourseId = userCourse._id
            };

        db.Students.Add(student);
        await db.SaveChangesAsync();

        return Results.Created<Models.Student>("", student);
    }
}
