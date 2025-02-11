
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
        var admin = await UserAccessor.GetUserAsync();

        var student = await Sender.Send(new AuthStudentCommand()
        {
            Email = "ivan.example1@urfu.me",
            PersonalNumber = "00000000",
            ChatId = "0",
        });

        var notification = await Sender.Send(new SendNotificationCommand()
        {
            Content = "test",
            StudentIds = [student.Value.Id],
        });

        var query = new GetNotificationByIdQuery()
        {
            NotificationId = notification.Value.Id
        };

        var result = await Sender.Send(query);

        Assert.True(result.IsSuccess);
        Assert.IsAssignableFrom<NotificationDto>(result.Value);
        Assert.Equal(notification.Value.Content, result.Value.Content);
        Assert.Equivalent(new StudentDto[] { student.Value }, notification.Value.Students);
    }

    [Fact]
    public async Task GetAdminById_ShouldFail_WhenBadInput()
    {
        var query = new GetNotificationByIdQuery()
        {
            NotificationId = 0
        };

        var result = await Sender.Send(query);

        Assert.True(result.IsFailed);
    }
}