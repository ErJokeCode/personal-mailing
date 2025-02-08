using Core.Infrastructure.Services;
using Core.Models;

namespace Core.Tests.Mocks;

public class UserAccessorMock : IUserAccessor
{
    public Task<Admin?> GetUserAsync()
    {
        return Task.FromResult<Admin?>(new Admin()
        {
            Email = "admin",
            UserName = "admin",
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
        });
    }
}