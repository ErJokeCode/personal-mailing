using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        var admin = await UserAccessor.GetUserAsync();

        var query = new GetAdminByIdQuery()
        {
            AdminId = admin!.Id
        };

        var result = await Sender.Send(query);

        Assert.True(result.IsSuccess);
        Assert.IsAssignableFrom<AdminDto>(result.Value);
        Assert.Equal(admin.Email, result.Value.Email);
    }

    [Fact]
    public async Task GetAdminById_ShouldFail_WhenBadInput()
    {
        var query = new GetAdminByIdQuery()
        {
            AdminId = Guid.NewGuid()
        };

        var result = await Sender.Send(query);

        Assert.True(result.IsFailed);
    }
}