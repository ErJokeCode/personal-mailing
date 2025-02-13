using System.Threading.Tasks;
using Core.Abstractions;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Infrastructure.Services;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly UserManager<Admin> _userManager;

    public UserAccessor(IHttpContextAccessor contextAccessor, UserManager<Admin> userManager)
    {
        _contextAccessor = contextAccessor;
        _userManager = userManager;
    }

    public async Task<Admin?> GetUserAsync()
    {
        var principal = _contextAccessor.HttpContext?.User;
        if (principal is null) return null;
        return await _userManager.GetUserAsync(principal);
    }
}