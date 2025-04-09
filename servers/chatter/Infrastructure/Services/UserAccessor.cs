using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Chatter.Abstractions.UserAccessor;
using Chatter.Models;

namespace Chatter.Infrastructure.Services;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _contextAccessor;

    public UserAccessor(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Task<Admin?> GetUserAsync()
    {
        var userInfo = _contextAccessor.HttpContext?.Request.Headers["x-user-info"].FirstOrDefault();

        if (userInfo is null)
        {
            return Task.FromResult<Admin?>(null);
        }

        return Task.FromResult(JsonSerializer.Deserialize<Admin?>(userInfo.ToString()));
    }
}