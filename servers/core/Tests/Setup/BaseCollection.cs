using System.Net.Http.Json;
using Core.Data;
using Core.Infrastructure.Services;
using Core.Models;
using Core.Routes.Admins.Commands;
using Core.Tests.Mocks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tests.Setup;

[Collection(nameof(SharedCollection))]
public abstract partial class BaseCollection : IAsyncLifetime
{
    protected readonly ApplicationFactory _appFactory;
    protected readonly IServiceScope _scope;

    protected readonly ISender Sender;
    protected readonly AppDbContext DbContext;
    protected readonly UserManager<Admin> UserManager;
    protected readonly SignInManager<Admin> SignInManager;

    protected readonly HttpClient HttpClient;

    protected BaseCollection(ApplicationFactory appFactory)
    {
        _appFactory = appFactory;
        _scope = _appFactory.Services.CreateScope();

        HttpClient = _appFactory.HttpClient;

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        UserManager = _scope.ServiceProvider.GetRequiredService<UserManager<Admin>>();
        SignInManager = _scope.ServiceProvider.GetRequiredService<SignInManager<Admin>>();
    }

    public async Task DisposeAsync()
    {
        await _appFactory.ResetDatabase();

        var createCommand = new CreateAdminCommand()
        {
            Email = "admin",
            Password = "admin",
        };

        await Sender.Send(createCommand);

        var loginCommand = new LoginAdminCommand()
        {
            Email = "admin",
            Password = "admin",
        };

        await HttpClient.PostAsJsonAsync("/admins/login", loginCommand);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}