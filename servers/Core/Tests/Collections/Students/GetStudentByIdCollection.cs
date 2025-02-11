using Core.Routes.Students.Commands;
using Core.Routes.Students.Dtos;
using Core.Routes.Students.Queries;
using Core.Tests.Setup;

namespace Tests.Collections.Students;

[Collection(nameof(SharedCollection))]
public class GetStudentByIdCollection : BaseCollection
{
    public GetStudentByIdCollection(ApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnStudent_WhenCorrectInput()
    {
        var command = new AuthStudentCommand()
        {
            Email = "ivan.example1@urfu.me",
            PersonalNumber = "00000000",
            ChatId = "0",
        };

        var student = await Sender.Send(command);

        var query = new GetStudentByIdQuery()
        {
            StudentId = student.Value.Id,
        };

        var result = await Sender.Send(query);

        Assert.True(result.IsSuccess);
        Assert.IsAssignableFrom<StudentDto>(result.Value);
        Assert.Equal(command.Email, result.Value.Email);
    }

    [Fact]
    public async Task GetStudentById_ShouldFail_WhenBadInput()
    {
        var query = new GetStudentByIdQuery()
        {
            StudentId = Guid.NewGuid(),
        };

        var result = await Sender.Send(query);

        Assert.True(result.IsFailed);
    }
}