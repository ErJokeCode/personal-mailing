using Core.Data;
using Core.Infrastructure.Services;
using Core.Tests.Mocks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tests.Setup;

[Collection("Tests")]
public abstract class BaseCollection
{
    private readonly IServiceScope _scope;

    protected readonly ISender Sender;
    protected readonly AppDbContext DbContext;
    protected readonly UserAccessorMock UserAccessor;

    protected BaseCollection(ApplicationFactory appFactory)
    {
        _scope = appFactory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        UserAccessor = (_scope.ServiceProvider.GetRequiredService<IUserAccessor>() as UserAccessorMock)!;
    }
}