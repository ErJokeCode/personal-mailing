
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
        var studentResult = await Sender.Send(authCommand);
        var admin = await UserAccessor.GetUserAsync();

        var command = new SendNotificationCommand()
        {
            Content = "test",
            StudentIds = [studentResult.Value.Id],
        };

        var result = await Sender.Send(command);

        Assert.True(result.IsSuccess);
        Assert.IsAssignableFrom<NotificationDto>(result.Value);

        Assert.Equivalent(new StudentDto[] { studentResult.Value }, result.Value.Students);
        Assert.Equal(admin!.Id, result.Value.Admin!.Id);
    }

    [Fact]
    public async Task SendNotification_ShouldFail_WhenBadInput()
    {
        var command = new SendNotificationCommand()
        {
            Content = "test",
            StudentIds = [Guid.NewGuid()],
        };

        var result = await Sender.Send(command);

        Assert.True(result.IsFailed);
    }
}