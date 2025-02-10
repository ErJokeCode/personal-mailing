
using Core.Routes.Admins.Commands;
using Core.Routes.Admins.Queries;
using Core.Tests.Setup;

namespace Core.Tests.Collections.Admins;

[Collection("Tests")]
public class CreateAdminCollection : BaseCollection
{
    public CreateAdminCollection(ApplicationFactory appFactory) : base(appFactory)
    {
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
        Assert.Equal(command.Email, queryResult.Value.Email);
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
}