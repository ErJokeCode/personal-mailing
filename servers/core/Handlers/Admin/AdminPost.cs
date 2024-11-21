
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

    public static async Task<IResult> AddAdmin(AdminDetails details, HttpContext context,
                                               UserManager<AdminUser> userManager, IUserStore<AdminUser> userStore,
                                               CoreDb db)
    {
        foreach (var permission in details.Permissions)
        {
            var allClaims = Permissions.All.Select(p => p.Claim).ToList();

            if (!allClaims.Contains(permission))
            {
                return Results.BadRequest($"{permission} is not a valid permission");
            }
        }

        details.Permissions.Add(Permissions.View.Claim);

        var created =
            await CreateAdmin(details.Email, details.Password, details.Permissions, userManager, userStore, db);
        if (!created)
        {
            return Results.BadRequest("Could not create an admin");
        }

        return Results.Ok();
    }
}