using Core.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tests.Setup;

public abstract class BaseTest : IClassFixture<TestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly AppDbContext DbContext;

    protected BaseTest(TestWebAppFactory appFactory)
    {
        _scope = appFactory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}