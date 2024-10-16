using System.Linq;
using Microsoft.AspNetCore.Builder;
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
        builder.Services.AddDbContext<UserDb>(options => options.UseNpgsql(connection));

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<UserDb>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }

        app.MapGet("/", (UserDb db) => db.Students);

        app.Run();
    }

    public class UserDb : DbContext
    {
        public DbSet<Student> Students => Set<Student>();

        public UserDb(DbContextOptions<UserDb> options) : base(options)
        {
        }
    }

    public class Student
    {
        public int Id { get; set; }
    }
}
