using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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

        app.Run();
    }

    private class AuthDetails
    {
        public string email { get; set; }
        public string personal_number { get; set; }
        public string chat_id { get; set; }
    }

    private static async Task<IResult> HandleAuth(AuthDetails details, Models.UserDb db)
    {
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://parser:8000"),
        };
        var query = new Dictionary<string, string>
        {
            ["email"] = details.email,
        };
        var response = await httpClient.GetAsync(QueryHelpers.AddQueryString("/course/by_email/", query));

        if (!response.IsSuccessStatusCode)
        {
            return Results.NotFound();
        }

        var result = await response.Content.ReadAsStringAsync();
        var userCourse = JsonSerializer.Deserialize<Models.UserCourse>(result);

        query = new Dictionary<string, string>
        {
            ["name"] = userCourse.name,
            ["sername"] = userCourse.sername,
            ["patronymic"] = userCourse.patronymic,
        };
        response = await httpClient.GetAsync(QueryHelpers.AddQueryString("/user_info/by_name/", query));

        if (!response.IsSuccessStatusCode)
        {
            return Results.NotFound();
        }

        result = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<Models.User>(result);

        if (details.personal_number != user.personal_number)
        {
            return Results.NotFound();
        }

        var student = new Models.Student()
        {
            Email = details.email,
            PersonalNumber = details.personal_number,
            ChatId = details.chat_id,
            UserId = user._id,
            UserCourseId = userCourse._id
        };

        db.Students.Add(student);
        await db.SaveChangesAsync();

        return Results.Created<Models.Student>("", student);
    }
}
