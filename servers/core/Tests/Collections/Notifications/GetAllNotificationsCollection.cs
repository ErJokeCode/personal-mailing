
using System.Net;
using System.Net.Http.Json;
using Core.Routes.Notifications.Commands;
using Core.Routes.Notifications.Dtos;
using Core.Routes.Notifications.Queries;
using Core.Routes.Students.Commands;
using Core.Routes.Students.Dtos;
using Core.Tests.Setup;

namespace Core.Tests.Collections.Notifications;

[Collection(nameof(SharedCollection))]
public class GetAllNotificationsCollection : BaseCollection
{
    public GetAllNotificationsCollection(ApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task GetAllNotifications_ShouldBeEmpty()
    {
        var response = await HttpClient.GetAsync("/notifications");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var notifications = await response.Content.ReadFromJsonAsync<IEnumerable<NotificationDto>>();

        Assert.NotNull(notifications);
        Assert.Empty(notifications);
    }

    [Fact]
    public async Task GetAllNotifications_ShouldHaveOneItem_AfterSendNotification()
    {
        var authCommand = new AuthStudentCommand()
        {
            Email = "ivan.example1@urfu.me",
            PersonalNumber = "00000000",
            ChatId = "0",
        };

        var studentResponse = await HttpClient.PostAsJsonAsync($"/students/auth", authCommand);
        var studentAuthed = await studentResponse.Content.ReadFromJsonAsync<StudentDto>();

        Assert.NotNull(studentAuthed);

        var sendCommand = new SendNotificationCommand()
        {
            Content = "test",
            StudentIds = [studentAuthed.Id],
        };

        await HttpClient.PostAsJsonAsync("/notifications", sendCommand);

        var response = await HttpClient.GetAsync("/notifications");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var notifications = await response.Content.ReadFromJsonAsync<IEnumerable<NotificationDto>>();

        Assert.NotNull(notifications);
        Assert.Single(notifications);
        Assert.All(notifications, (n) =>
        {
            Assert.Empty(n.Students);
            Assert.NotNull(n.Admin);
        });
    }
}