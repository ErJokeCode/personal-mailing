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
    public async Task GetAllStudents_ShouldBeStudentArray()
    {
        var query = new GetAllStudentsQuery();

        var result = await Sender.Send(query);

        Assert.IsAssignableFrom<IEnumerable<StudentDto>>(result);
        Assert.All(result, (s) => Assert.NotNull(s.Info));
    }

    [Fact]
    public async Task GetAllStudents_ShouldBeEmtpy()
    {
        var query = new GetAllStudentsQuery();

        var result = await Sender.Send(query);

        Assert.Empty(result);
    }
}