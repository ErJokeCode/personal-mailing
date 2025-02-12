using System.Data.Common;
using System.Net.Http.Json;
using Core.External.TelegramBot;
using Core.Infrastructure.Services;
using Core.Routes.Admins.Commands;
using Core.Tests.Mocks;
using DotNet.Testcontainers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace Core.Tests.Setup;

public class ApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer _postgresContainer = null!;

    private MongoDbContainer _mongoContainer = null!;
    private IFutureDockerImage _mongoSeedImage = null!;
    private IContainer _mongoSeedContainer = null!;

    private IFutureDockerImage _parserImage = null!;
    private IContainer _parserContainer = null!;

    private DbConnection _dbConnection = null!;
    private Respawner _respawner = null!;

    public HttpClient HttpClient { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await InitializePostgres();
        await InitializeParser();
        await InitializeRespawn();
    }

    private async Task InitializePostgres()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres")
            .WithDatabase("coredb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _postgresContainer.StartAsync();
    }

    private async Task InitializeParser()
    {
        var mongoNetwork = new NetworkBuilder()
          .WithName("mongo-network")
          .Build();

        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo")
            .WithNetwork(mongoNetwork)
            .WithUsername("")
            .WithPassword("")
            .Build();

        await _mongoContainer.StartAsync();

        var projectPath = CommonDirectoryPath.GetProjectDirectory().DirectoryPath;
        var seedsPath = Path.GetFullPath(Path.Combine(projectPath, "Seeds"));

        _mongoSeedImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(seedsPath)
            .WithDockerfile("Dockerfile")
            .Build();

        await _mongoSeedImage.CreateAsync();

        _mongoSeedContainer = new ContainerBuilder()
            .WithImage(_mongoSeedImage)
            .WithNetwork(mongoNetwork)
            .WithEnvironment("DATABASE_HOST", _mongoContainer.Name)
            .Build();

        await _mongoSeedContainer.StartAsync();

        var slnPath = CommonDirectoryPath.GetSolutionDirectory().DirectoryPath;
        var parserPath = Path.GetFullPath(Path.Combine(slnPath, "..", "parser"));

        _parserImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(parserPath)
            .WithDockerfile("Dockerfile")
            .Build();

        await _parserImage.CreateAsync();

        _parserContainer = new ContainerBuilder()
            .WithImage(_parserImage)
            .WithPortBinding(8000, true)
            .WithNetwork(mongoNetwork)
            .WithEnvironment("MGO_HOST", _mongoContainer.Name.Replace("/", ""))
            .WithEnvironment("MGO_PORT", "27017")
            .WithEnvironment("MGO_NAME_DB", "personal-mailing")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8000))
            .Build();

        await _parserContainer.StartAsync();
    }

    private async Task InitializeRespawn()
    {
        _dbConnection = new NpgsqlConnection(_postgresContainer.GetConnectionString());

        HttpClient = CreateClient();

        await HttpClient.PostAsJsonAsync("/admins/login", new LoginAdminCommand()
        {
            Email = "admin",
            Password = "admin",
        });

        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            SchemasToInclude = ["public"],
            DbAdapter = DbAdapter.Postgres
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable(
            "ConnectionStrings:Database",
            _postgresContainer!.GetConnectionString()
        );

        Environment.SetEnvironmentVariable(
            "ConnectionStrings:Parser",
            "http://" + _parserContainer.Hostname + ":" + _parserContainer.GetMappedPublicPort(8000)
        );

        builder.ConfigureTestServices(services =>
        {
            var botDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(ITelegramBot));

            if (botDescriptor is not null)
            {
                services.Remove(botDescriptor);
            }

            services.AddScoped<ITelegramBot, TelegramBotMock>();
        });
    }

    public async Task ResetDatabase()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.StopAsync();
        await _dbConnection.DisposeAsync();
        await _mongoContainer.StopAsync();
        await _parserImage.DisposeAsync();
        await _parserContainer.DisposeAsync();
    }
}