using Core.Data;
using Core.Infrastructure.Services;
using Core.Models;

namespace Core.Tests.Mocks;

public class UserAccessorMock : IUserAccessor
{
    private readonly AppDbContext _db;

    private Admin _admin;

    public UserAccessorMock(AppDbContext db)
    {
        _db = db;
        _admin = _db.Users.SingleOrDefault(a => a.Email == "admin")!;
    }

    public void InjectUser(Admin admin)
    {
        _admin = admin;
    }

    public Task<Admin?> GetUserAsync()
    {
        return Task.FromResult<Admin?>(_admin);
    }
}