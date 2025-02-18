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
public class GetNotificationByIdCollection : BaseCollection
{
    public GetNotificationByIdCollection(ApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task GetNotificationById_ShouldReturnNotification_WhenCorrectInput()
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

        var notificationResponse = await HttpClient.PostAsJsonAsync("/notifications", sendCommand);
        var notificationSent = await notificationResponse.Content.ReadFromJsonAsync<NotificationDto>();

        Assert.NotNull(notificationSent);

        var response = await HttpClient.GetAsync($"/notifications/{notificationSent.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var notification = await response.Content.ReadFromJsonAsync<NotificationDto>();

        Assert.NotNull(notification);
        Assert.Equal(notificationSent.Id, notification.Id);
        Assert.NotEmpty(notification.Students);
        Assert.NotNull(notification.Admin);
    }

    [Fact]
    public async Task GetAdminById_ShouldFail_WhenBadInput()
    {
        var response = await HttpClient.GetAsync($"/notification/{0}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}