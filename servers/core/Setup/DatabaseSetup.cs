using System;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Core.Data;

namespace Core.Setup;

public static class DatabaseSetup
{
    public static void SetupDatabase(this IServiceCollection services)
    {
        var host = Environment.GetEnvironmentVariable("POSTGRES_URL");
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER");

        var connection = $"Host={host};Port={5432};Database={database};Username={user};Password={password};Include Error Detail=True";

        services.AddDbContext<AppDbContext>(o =>
        {
            o.UseNpgsql(connection);
            o.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
        });
    }
}