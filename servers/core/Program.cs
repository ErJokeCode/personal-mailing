using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Models;
using Shared.Messages;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading;

namespace Core;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        await builder.ConfigureServices();

        var app = builder.Build();
        await app.InitialzieServices();

        app.MapPost("/core/auth", Handlers.HandleAuth);
        app.MapGet("/core/{id}/courses", Handlers.HandleCourses);

        app.Run();
    }

    public static async Task ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(
            options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins(new string[] { "http://localhost:5010" }).AllowAnyHeader().AllowAnyMethod();
                    });
            });

        builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        builder.Services.AddMassTransit(o =>
                                        {
                                            o.UsingRabbitMq((context, cfg) =>
                                                            {
                                                                cfg.Host("rabbitmq", "/",
                                                                         h =>
                                                                         {
                                                                             h.Username("guest");
                                                                             h.Password("guest");
                                                                         });

                                                                cfg.ConfigureEndpoints(context);
                                                            });
                                        });

        var connection = builder.Configuration.GetConnectionString("Database");
        builder.Services.AddDbContext<CoreDb>(options => options.UseNpgsql(connection));

        builder.Services.AddAuthorization(
            options =>
            { options.AddPolicy("CustomPolicy", policy => policy.RequireAuthenticatedUser()); });

        builder.Services
            .AddIdentityApiEndpoints<ApplicationUser>(o =>
                                                      {
                                                          o.Password.RequireDigit = false;
                                                          o.Password.RequireNonAlphanumeric = false;
                                                          o.Password.RequireLowercase = false;
                                                          o.Password.RequireUppercase = false;
                                                          o.Password.RequiredUniqueChars = 0;
                                                          o.Password.RequiredLength = 1;
                                                      })
            .AddEntityFrameworkStores<CoreDb>();
    }

    public static async Task InitialzieServices(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<CoreDb>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var userStore = services.GetRequiredService<IUserStore<ApplicationUser>>();
            var emailStore = (IUserEmailStore<ApplicationUser>)userStore;

            var email = "admin";
            var password = "admin";
            var user = new ApplicationUser();

            await userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await emailStore.SetEmailAsync(user, email, CancellationToken.None);
            var result = await userManager.CreateAsync(user, password);
        }

        app.UseCors();
        app.MapCustomIdentityApi<ApplicationUser>();
        app.MapReverseProxy();
    }
}
