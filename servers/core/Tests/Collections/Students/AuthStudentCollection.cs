
using System.Net;
using System.Net.Http.Json;
using Core.Routes.Students.Commands;
using Core.Routes.Students.Dtos;
using Core.Routes.Students.Maps;
using Core.Tests.Setup;
using Microsoft.EntityFrameworkCore;

namespace Core.Tests.Setup
{
    public partial class BaseCollection
    {
        public async Task<StudentDto> AuthStudent(AuthStudentCommand command)
        {
            var response = await HttpClient.PostAsJsonAsync("/students/auth", command);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var student = await response.Content.ReadFromJsonAsync<StudentDto>();

            Assert.NotNull(student);
            Assert.Equal(command.Email, student.Email);

            var studentDb = await DbContext.Students.FindAsync(student.Id);

            Assert.NotNull(studentDb);

            var studentMapper = new StudentMapper();
            var studentDto = studentMapper.Map(studentDb);

            Assert.Equivalent(student, studentDto);

            return student;
        }
    }
}

namespace Core.Tests.Collections.Students
{
    [Collection(nameof(SharedCollection))]
    public class AuthStudentCollection : BaseCollection
    {
        public AuthStudentCollection(ApplicationFactory appFactory) : base(appFactory)
        {
        }

        [Fact]
        public async Task AuthStudent_ShouldAddStudent_WhenCorrectInput()
        {
            var authCommand = new AuthStudentCommand()
            {
                Email = "ivan.example1@urfu.me",
                PersonalNumber = "00000000",
                ChatId = "0",
            };

            await AuthStudent(authCommand);
        }

        [Fact]
        public async Task AuthStudent_ShouldFail_WhenBadInput()
        {
            var authCommand = new AuthStudentCommand()
            {
                Email = "email@urfu.me",
                PersonalNumber = "123",
                ChatId = "10",
            };

            var response = await HttpClient.PostAsJsonAsync("/students/auth", authCommand);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var studentsDb = await DbContext.Students.ToListAsync();

            Assert.Empty(studentsDb);
        }
    }
}