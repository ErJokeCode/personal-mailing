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
using Core.Models;
using Shared.Core;
using Shared.Messages;
using NServiceBus;

namespace Core;

public static class Program
{
    public static IEndpointInstance _endpoint = null;

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connection = builder.Configuration.GetConnectionString("Database");
        builder.Services.AddDbContext<CoreDb>(options => options.UseNpgsql(connection));

        var app = builder.Build();

        var endpointConfiguration = new EndpointConfiguration("Core");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        transport.ConnectionString("host=rabbitmq");

        var routing = transport.Routing();
        routing.RouteToEndpoint(typeof(NewStudentAuth), "Notify");

        var endpointInstance = await NServiceBus.Endpoint.Start(endpointConfiguration);
        _endpoint = endpointInstance;

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<CoreDb>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }

        app.MapPost("/auth", HandleAuth);
        app.MapGet("/{id}/courses", HandleCourses);

        app.Run();

        await endpointInstance.Stop();
    }

    private static async Task<IResult> HandleCourses(Guid id, CoreDb db)
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
        var userCourse = JsonSerializer.Deserialize<UserCourse>(result, serializerSettings);

        return Results.Ok(userCourse.Courses);
    }

    private static async Task<IResult> HandleAuth(AuthDetails details, CoreDb db)
    {
        var student = db.Students.SingleOrDefault(s => s.Email == details.Email);

        if (student != null)
        {
            return Results.Ok(student);
        }

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
        var userCourse = JsonSerializer.Deserialize<UserCourse>(result, serializerSettings);

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
        var user = JsonSerializer.Deserialize<User>(result, serializerSettings);

        if (details.PersonalNumber != user.PersonalNumber)
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

        await _endpoint.Send(new NewStudentAuth() { Student = student });

        return Results.Created<Student>("", student);
    }
}
