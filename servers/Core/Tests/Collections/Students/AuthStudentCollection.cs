
using Core.Routes.Students.Commands;
using Core.Tests.Setup;

namespace Core.Tests.Collections.Students;

[Collection("Tests")]
public class AuthStudentCollection : BaseCollection
{
    public AuthStudentCollection(ApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task AuthStudent_ShouldAuth_WhenCorrectInput()
    {
        var command = new AuthStudentCommand()
        {
            Email = "ivan.example1@urfu.me",
            PersonalNumber = "00000000",
            ChatId = "0",
        };
        var result = await Sender.Send(command);
        Assert.True(result.IsSuccess);
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
        Assert.True(result.IsFailed);
    }
}