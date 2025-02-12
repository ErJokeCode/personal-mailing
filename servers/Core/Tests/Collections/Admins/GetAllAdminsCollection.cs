using System.Net;
using System.Net.Http.Json;
using Core.Models;
using Core.Routes.Admins.Commands;
using Core.Routes.Admins.Dtos;
using Core.Tests.Setup;
using Microsoft.AspNetCore.Http;

namespace Core.Tests.Collections.Admins;

[Collection(nameof(SharedCollection))]
public class GetAllAdminsCollection : BaseCollection
{
    public GetAllAdminsCollection(ApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task GetAllAdmins_ShouldContainOneItem()
    {
        var response = await HttpClient.GetAsync("/admins");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var admins = await response.Content.ReadFromJsonAsync<IEnumerable<AdminDto>>();

        Assert.NotNull(admins);
        Assert.NotEmpty(admins);
        Assert.Single(admins);
    }

    [Fact]
    public async Task GetAllAdmins_ShouldContainMultipleItems_AfterAdminCreate()
    {
        await CreateAdmin(new CreateAdminCommand()
        {
            Email = "newadmin",
            Password = "newadmin",
        });

        var response = await HttpClient.GetFromJsonAsync<IEnumerable<AdminDto>>("/admins");

        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.Equal(2, response.Count());
    }
}