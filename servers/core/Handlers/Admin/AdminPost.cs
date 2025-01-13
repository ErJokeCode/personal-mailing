
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Identity;
using Core.Models;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Handlers;

public static partial class AdminHandler
{
    public class AdminDetails
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Permissions { get; set; } = [];
    }

    // NOTE right now creates admins with all permissions
    // TODO when frontend is done, change to specific permissions
    public static async Task<IResult> AddAdmin(AdminDetails details, HttpContext context,
                                               UserManager<AdminUser> userManager, IUserStore<AdminUser> userStore,
                                               CoreDb db)
    {
        var allClaims = Permissions.All.Select(p => p.Claim).ToList();

        foreach (var permission in details.Permissions)
        {
            if (!allClaims.Contains(permission))
            {
                return Results.BadRequest($"{permission} is not a valid permission");
            }
        }

        details.Permissions.Add(Permissions.View.Claim);

        var created = await CreateAdmin(details.Email, details.Password, Permissions.All.Select(p => p.Claim).ToList(),
                                        userManager, userStore, db);
        if (!created)
        {
            return Results.BadRequest("Could not create an admin");
        }

        return Results.Ok();
    }
}
