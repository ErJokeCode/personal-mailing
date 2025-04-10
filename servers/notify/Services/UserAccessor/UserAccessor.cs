using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Notify.Data;
using Notify.Features.Notifications.DTOs;
using Notify.Models;

namespace Notify.Services.UserAccessor;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly AppDbContext _db;

    public UserAccessor(IHttpContextAccessor contextAccessor, AppDbContext db)
    {
        _contextAccessor = contextAccessor;
        _db = db;
    }

    public async Task<Admin?> GetUserAsync()
    {
        var userInfo = _contextAccessor.HttpContext?.Request.Headers["x-user-info"].FirstOrDefault();

        if (userInfo is null)
        {
            return null;
        }

        var user = JsonSerializer.Deserialize<AdminDto?>(userInfo.ToString());

        if (user is null)
        {
            return null;
        }

        var admin = await _db.Users.FindAsync(user.Id);

        if (admin is null)
        {
            return null;
        }

        return admin;
    }
}