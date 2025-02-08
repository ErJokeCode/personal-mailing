
using Core.Routes.Students.Commands;
using Core.Tests.Setup;

namespace Core.Tests;

[Collection("Tests")]
public class StudentTests : BaseTest
{
    public StudentTests(TestWebAppFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task AuthStudent_ShouldFail_WhenBadInput()
    {
        var command = new AuthStudentCommand()
        {
            Email = "email@urfu.me",
            PersonalNumber = "123",
            ChatId = "10",
        };
        var result = await Sender.Send(command);
        Assert.False(result.IsSuccess);
    }
}