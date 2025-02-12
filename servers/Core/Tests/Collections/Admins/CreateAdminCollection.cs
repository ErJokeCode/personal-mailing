using System.Net;
using System.Net.Http.Json;
using Core.Routes.Admins.Commands;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Maps;
using Core.Tests.Setup;
using Microsoft.EntityFrameworkCore;

namespace Core.Tests.Setup
{
    public partial class BaseCollection
    {
        public async Task<AdminDto> CreateAdmin(CreateAdminCommand command)
        {
            var response = await HttpClient.PostAsJsonAsync("/admins", command);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var admin = await response.Content.ReadFromJsonAsync<AdminDto>();

            Assert.NotNull(admin);
            Assert.Equal(command.Email, admin.Email);

            var adminDb = await DbContext.Users.FindAsync(admin.Id);

            Assert.NotNull(adminDb);

            var adminMapper = new AdminMapper();
            var adminDto = adminMapper.Map(adminDb);

            Assert.Equivalent(admin, adminDto);

            return admin;
        }
    }
}

namespace Core.Tests.Collections.Admins
{
    [Collection(nameof(SharedCollection))]
    public class CreateAdminCollection : BaseCollection
    {
        public CreateAdminCollection(ApplicationFactory appFactory) : base(appFactory)
        {
        }

        [Fact]
        public async Task CreateAdmin_ShouldAddAdmin_WhenCorrectInput()
        {
            var createCommand = new CreateAdminCommand()
            {
                Email = "newadmin",
                Password = "newadmin",
            };

            await CreateAdmin(createCommand);
        }

        [Fact]
        public async Task CreateAdmin_ShouldFail_WhenBadInput()
        {
            var createCommand = new CreateAdminCommand()
            {
                Email = "",
                Password = "",
            };

            var response = await HttpClient.PostAsJsonAsync("/admins", createCommand);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var adminsDb = await DbContext.Users.ToListAsync();

            Assert.Empty(adminsDb);
        }
    }
}