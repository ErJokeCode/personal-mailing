using Core.Infrastructure.Services;
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
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace Core.Tests.Setup;

public class ApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _dbContainer;

    private MongoDbContainer? _mongoContainer;
    private IFutureDockerImage? _mongoSeedImage;
    private IContainer? _mongoSeedContainer;

    private IFutureDockerImage? _parserImage;
    private IContainer? _parserContainer;

    public async Task InitializeAsync()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres")
            .WithDatabase("coredb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _dbContainer.StartAsync();

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

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable(
            "ConnectionStrings:Database",
            _dbContainer!.GetConnectionString()
        );

        Environment.SetEnvironmentVariable(
            "ConnectionStrings:Parser",
            "http://" + _parserContainer!.Hostname + ":" + _parserContainer!.GetMappedPublicPort(8000)
        );

        builder.ConfigureTestServices(services =>
        {
            var userDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IUserAccessor));

            if (userDescriptor is not null)
            {
                services.Remove(userDescriptor);
            }

            services.AddScoped<IUserAccessor, UserAccessorMock>();
        });
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer!.StopAsync();
        await _mongoContainer!.StopAsync();
        await _parserImage!.DisposeAsync();
        await _parserContainer!.DisposeAsync();
    }
}