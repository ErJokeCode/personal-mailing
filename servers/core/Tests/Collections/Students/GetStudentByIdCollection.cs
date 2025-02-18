using System.Net;
using System.Net.Http.Json;
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
        var authCommand = new AuthStudentCommand()
        {
            Email = "ivan.example1@urfu.me",
            PersonalNumber = "00000000",
            ChatId = "0",
        };

        var studentAuthed = await HttpClient.PostFromJsonAsync<StudentDto, AuthStudentCommand>("/students/auth", authCommand);

        var response = await HttpClient.GetAsync($"/students/{studentAuthed.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var student = await response.Content.ReadFromJsonAsync<StudentDto>();

        Assert.NotNull(student);
        Assert.Equivalent(studentAuthed, student);
    }

    [Fact]
    public async Task GetStudentById_ShouldFail_WhenBadInput()
    {
        var response = await HttpClient.GetAsync($"/students/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}