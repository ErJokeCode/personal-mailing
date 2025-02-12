using System.Net;
using System.Net.Http.Json;
using Core.Routes.Students.Commands;
using Core.Routes.Students.Dtos;
using Core.Routes.Students.Queries;
using Core.Tests.Setup;

namespace Tests.Collections.Students;

[Collection(nameof(SharedCollection))]
public class GetAllStudentsCollection : BaseCollection
{
    public GetAllStudentsCollection(ApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task GetAllStudents_ShouldBeEmpty()
    {
        var response = await HttpClient.GetAsync("/students");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var admins = await response.Content.ReadFromJsonAsync<IEnumerable<StudentDto>>();

        Assert.NotNull(admins);
        Assert.Empty(admins);
    }

    [Fact]
    public async Task GetAllStudents_ShouldHaveOneItem_AfterStudentAuth()
    {
        var authCommand = new AuthStudentCommand()
        {
            Email = "ivan.example1@urfu.me",
            PersonalNumber = "00000000",
            ChatId = "0",
        };

        await AuthStudent(authCommand);

        var admins = await HttpClient.GetFromJsonAsync<IEnumerable<StudentDto>>("/students");

        Assert.NotNull(admins);
        Assert.Single(admins);
    }
}