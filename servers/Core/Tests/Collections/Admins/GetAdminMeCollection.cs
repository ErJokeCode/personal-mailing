using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Commands;
using Core.Tests.Setup;
using System.Net.Http.Json;
using System.Net;

namespace Core.Tests.Collections.Admins;

[Collection(nameof(SharedCollection))]
public class GetAdminMeCollection : BaseCollection
{
    public GetAdminMeCollection(ApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task GetAdminMe_ShouldReturnMe()
    {
        var response = await HttpClient.GetAsync("/admins/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var adminMe = await response.Content.ReadFromJsonAsync<AdminDto>();

        Assert.NotNull(adminMe);
        Assert.Equal("admin", adminMe.Email);
    }

    [Fact]
    public async Task GetAdminMe_ShouldReturnMe_WhenLoggedInAsDifferentUser()
    {
        var createCommand = new CreateAdminCommand()
        {
            Email = "newadmin",
            Password = "newadmin",
        };

        await CreateAdmin(createCommand);

        var loginCommand = new LoginAdminCommand()
        {
            Email = "newadmin",
            Password = "newadmin",
        };

        await LoginAsAdmin(loginCommand);

        var response = await HttpClient.GetAsync("/admins/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var adminMe = await response.Content.ReadFromJsonAsync<AdminDto>();

        Assert.NotNull(adminMe);
        Assert.Equal(loginCommand.Email, adminMe.Email);
    }
}