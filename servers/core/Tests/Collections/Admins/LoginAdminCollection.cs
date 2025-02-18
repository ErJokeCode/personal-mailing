using System.Net;
using System.Net.Http.Json;
using Core.Routes.Admins.Commands;
using Core.Tests.Setup;

namespace Core.Tests.Setup
{
    public partial class BaseCollection
    {
        public async Task LoginAsAdmin(LoginAdminCommand command)
        {
            var response = await HttpClient.PostAsJsonAsync("/admins/login", command);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

namespace Core.Tests.Collections.Admins
{
    [Collection(nameof(SharedCollection))]
    public class LoginAdminCollection : BaseCollection
    {
        public LoginAdminCollection(ApplicationFactory appFactory) : base(appFactory)
        {
        }

        [Fact]
        public async Task LoginAdmin_ShouldLogin_WhenCorrectInput()
        {
            var createCommand = new CreateAdminCommand()
            {
                Email = "newadmin",
                Password = "newadmin",
            };

            await CreateAdmin(createCommand);

            var loginCommand = new LoginAdminCommand()
            {
                Email = "newadmin",
                Password = "newadmin",
            };

            await LoginAsAdmin(loginCommand);
        }

        [Fact]
        public async Task LoginAdmin_ShouldFail_WhenBadInput()
        {
            var loginCommand = new LoginAdminCommand()
            {
                Email = "newadmin",
                Password = "newadmin",
            };

            var response = await HttpClient.PostAsJsonAsync("/admins/login", loginCommand);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}