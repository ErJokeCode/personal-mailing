using Core.Models;
using Core.Routes.Admins.Commands;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Queries;
using Core.Tests.Setup;
using Microsoft.AspNetCore.Authentication;

namespace Core.Tests;

[Collection("Tests")]
public class AdminTests : BaseTest
{
    public AdminTests(TestWebAppFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task GetAllAdmins_ShouldBeAdminArray()
    {
        var query = new GetAllAdminsQuery();

        var result = await Sender.Send(query);

        Assert.IsAssignableFrom<IEnumerable<AdminDto>>(result);
    }

    [Fact]
    public async Task CreateAdmin_ShouldCreateAdmin_WhenCorrectInput()
    {
        var command = new CreateAdminCommand()
        {
            Email = "test",
            Password = "test",
        };
        var result = await Sender.Send(command);
        Assert.True(result.IsSuccess);

        var admin = result.Value;
        var query = new GetAdminByIdQuery(admin.Id);
        var queryResult = await Sender.Send(query);
        Assert.True(queryResult.IsSuccess);
    }

    [Fact]
    public async Task CreateAdmin_ShouldFail_WhenBadInput()
    {
        var command = new CreateAdminCommand()
        {
            Email = "",
            Password = "",
        };
        var result = await Sender.Send(command);
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task GetAdminMe_ShouldReturnMe()
    {
        var query = new GetAdminMeQuery();
        var result = await Sender.Send(query);
        Assert.IsAssignableFrom<AdminDto>(result.Value);
    }
}