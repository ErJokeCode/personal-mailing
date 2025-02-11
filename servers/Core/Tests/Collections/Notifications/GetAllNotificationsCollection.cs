
using Core.Routes.Notifications.Dtos;
using Core.Routes.Notifications.Queries;
using Core.Tests.Setup;

namespace Core.Tests.Collections.Notifications;

[Collection(nameof(SharedCollection))]
public class GetAllNotificationsCollection : BaseCollection
{
    public GetAllNotificationsCollection(ApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task GetAllNotifications_ShouldBeNotificationArray()
    {
        var query = new GetAllNotificationsQuery();

        var result = await Sender.Send(query);

        Assert.IsAssignableFrom<IEnumerable<NotificationDto>>(result);
        Assert.All(result, (n) => Assert.NotNull(n.Admin));
        Assert.All(result, (n) => Assert.Empty(n.Students));
    }
}