using Core.Data;
using Core.External.Parser;
using Core.Infrastructure.Services;
using Core.Tests.Mocks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace Core.Tests.Setup;

public class TestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _dbContainer;
    private MongoDbContainer? _mongoContainer;
    private IFutureDockerImage? _parserImage;
    private IContainer? _parserContainer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var dbDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbDescriptor is not null)
            {
                services.Remove(dbDescriptor);
            }

            services.AddDbContext<AppDbContext>(o =>
            {
                o.UseNpgsql(_dbContainer!.GetConnectionString());
            });

            var userDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IUserAccessor));

            if (userDescriptor is not null)
            {
                services.Remove(userDescriptor);
            }

            services.AddScoped<IUserAccessor, UserAccessorMock>();

            var parserDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(ParserOptions));

            if (parserDescriptor is not null)
            {
                services.Remove(parserDescriptor);
            }

            services.Configure<ParserOptions>(o =>
            {
                o.ParserUrl = "http://localhost:" + _parserContainer!.GetMappedPublicPort("8000");
            });
            services.AddScoped<IParser, Parser>();
        });
    }

    public async Task InitializeAsync()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres")
            .WithDatabase("coredb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _dbContainer.StartAsync();

        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo")
            .WithUsername("")
            .WithPassword("")
            .WithWaitStrategy(Wait.ForUnixContainer())
            .Build();

        await _mongoContainer.StartAsync();

        var path = CommonDirectoryPath.GetSolutionDirectory().DirectoryPath;
        path = Path.GetFullPath(Path.Combine(path, "..", "parser"));
        var mongoPort = _mongoContainer.GetConnectionString()[^6..^1];

        _parserImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(path)
            .WithDockerfile("Dockerfile")
            .Build();

        await _parserImage.CreateAsync();

        _parserContainer = new ContainerBuilder()
            .WithImage(_parserImage)
            .WithPortBinding("8000")
            .WithEnvironment("MGO_HOST", "localhost")
            .WithEnvironment("MGO_PORT", mongoPort)
            .WithEnvironment("MGO_NAME_DB", "personal_mailing")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8000))
            .Build();

        await _parserContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer!.StopAsync();
        await _mongoContainer!.StopAsync();
        await _parserImage!.DisposeAsync();
        await _parserContainer!.DisposeAsync();
    }
}