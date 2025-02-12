
using System.Net;
using System.Net.Http.Json;
using Core.Routes.Admins.Dtos;
using Core.Routes.Notifications.Commands;
using Core.Routes.Notifications.Dtos;
using Core.Routes.Students.Commands;
using Core.Routes.Students.Dtos;
using Core.Routes.Students.Queries;
using Core.Tests.Setup;

namespace Core.Tests.Collections.Notifications;

[Collection(nameof(SharedCollection))]
public class SendNotificationCollection : BaseCollection
{
    public SendNotificationCollection(ApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task SendNotification_ShouldSendNotification_WhenCorrectInput()
    {
        var authCommand = new AuthStudentCommand()
        {
            Email = "ivan.example1@urfu.me",
            PersonalNumber = "00000000",
            ChatId = "0",
        };

        var studentResponse = await HttpClient.PostAsJsonAsync($"/students/auth", authCommand);
        var studentAuthed = await studentResponse.Content.ReadFromJsonAsync<StudentDto>();
        var adminMe = await HttpClient.GetFromJsonAsync<AdminDto>($"/admins/me");

        Assert.NotNull(studentAuthed);
        Assert.NotNull(adminMe);

        var sendCommand = new SendNotificationCommand()
        {
            Content = "test",
            StudentIds = [studentAuthed.Id],
        };

        var response = await HttpClient.PostAsJsonAsync("/notifications", sendCommand);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var notification = await response.Content.ReadFromJsonAsync<NotificationDto>();

        Assert.NotNull(notification);
        Assert.Equal(sendCommand.Content, notification.Content);

        Assert.NotEmpty(notification.Students);
        Assert.Equivalent(new StudentDto[] { studentAuthed }, notification.Students);

        Assert.NotNull(notification.Admin);
        Assert.Equivalent(notification.Admin, adminMe);
    }

    // TODO Should transaction, try to send to who you can, if error, display that

    // [Fact]
    // public async Task SendNotification_ShouldFail_WhenBadInput()
    // {
    //     var command = new SendNotificationCommand()
    //     {
    //         Content = "test",
    //         StudentIds = [Guid.NewGuid()],
    //     };

    //     var result = await Sender.Send(command);

    //     Assert.True(result.IsFailed);
    // }
}