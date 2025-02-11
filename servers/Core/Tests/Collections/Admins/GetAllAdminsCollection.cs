
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Queries;
using Core.Tests.Setup;

namespace Core.Tests.Collections.Admins;

[Collection(nameof(SharedCollection))]
public class GetAllAdminsCollection : BaseCollection
{
    public GetAllAdminsCollection(ApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task GetAllAdmins_ShouldBeAdminArray()
    {
        var query = new GetAllAdminsQuery();

        var result = await Sender.Send(query);

        Assert.IsAssignableFrom<IEnumerable<AdminDto>>(result);
    }
}