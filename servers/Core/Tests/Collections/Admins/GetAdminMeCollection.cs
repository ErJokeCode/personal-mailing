
using Core.Routes.Admins.Queries;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Commands;
using Core.Tests.Setup;
using Microsoft.EntityFrameworkCore;

namespace Core.Tests.Collections.Admins;

[Collection(nameof(SharedCollection))]
public class GetAdminMeCollection : BaseCollection
{
    public GetAdminMeCollection(ApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task GetAdminMe_ShouldReturnMainAdmin()
    {
        var query = new GetAdminMeQuery();
        var result = await Sender.Send(query);
        Assert.IsAssignableFrom<AdminDto>(result.Value);
        Assert.Equal("admin", result.Value.Email);
    }

    [Fact]
    public async Task GetAdminMe_ShouldReturnMe_WhenLoggedInAsDifferentUser()
    {
        var command = new CreateAdminCommand()
        {
            Email = "newadmin",
            Password = "newadmin",
        };
        var createResult = await Sender.Send(command);
        Assert.True(createResult.IsSuccess);

        var newAdmin = await DbContext.Users.SingleOrDefaultAsync(a => a.Id == createResult.Value.Id);
        Assert.NotNull(newAdmin);

        UserAccessor.InjectUser(newAdmin);

        var query = new GetAdminMeQuery();
        var getResult = await Sender.Send(query);
        Assert.IsAssignableFrom<AdminDto>(getResult.Value);
        Assert.Equal(newAdmin.Id, getResult.Value.Id);
    }
}