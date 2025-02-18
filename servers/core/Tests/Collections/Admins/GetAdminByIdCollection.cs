using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Core.Routes.Admins.Commands;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Queries;
using Core.Tests.Setup;

namespace Tests.Collections.Admins;

[Collection(nameof(SharedCollection))]
public class GetAdminByIdCollection : BaseCollection
{
    public GetAdminByIdCollection(ApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task GetAdminById_ShouldReturnAdmin_WhenCorrectInput()
    {
        var newAdmin = await CreateAdmin(new CreateAdminCommand()
        {
            Email = "newadmin",
            Password = "newadmin",
        });

        var response = await HttpClient.GetAsync($"/admins/{newAdmin.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var admin = await response.Content.ReadFromJsonAsync<AdminDto>();

        Assert.NotNull(admin);
        Assert.Equal(newAdmin.Id, admin.Id);
    }

    [Fact]
    public async Task GetAdminById_ShouldFail_WhenBadInput()
    {
        var response = await HttpClient.GetAsync($"/admins/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}