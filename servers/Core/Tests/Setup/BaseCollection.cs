using Core.Data;
using Core.Infrastructure.Services;
using Core.Models;
using Core.Routes.Admins.Commands;
using Core.Tests.Mocks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tests.Setup;

[Collection(nameof(SharedCollection))]
public abstract class BaseCollection : IAsyncLifetime
{
    protected readonly ApplicationFactory _appFactory;
    protected readonly IServiceScope _scope;

    protected readonly ISender Sender;
    protected readonly AppDbContext DbContext;
    protected readonly UserAccessorMock UserAccessor;

    protected BaseCollection(ApplicationFactory appFactory)
    {
        _appFactory = appFactory;
        _scope = _appFactory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        UserAccessor = (_scope.ServiceProvider.GetRequiredService<IUserAccessor>() as UserAccessorMock)!;
    }

    public async Task DisposeAsync()
    {
        await _appFactory.ResetDatabase();

        var command = new CreateAdminCommand()
        {
            Email = "admin",
            Password = "admin",
        };

        await Sender.Send(command);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}