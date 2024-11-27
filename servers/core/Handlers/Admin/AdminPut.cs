using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Identity;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Handlers;

public static partial class AdminHandler
{
    public static async Task<IResult> UpdatePermissions(string adminId, List<string> permissions, CoreDb db,
                                                        UserManager<AdminUser> userManager)
    {
        var admin = await db.Users.FindAsync(adminId);

        if (admin == null)
        {
            return Results.NotFound("Admin not found");
        }

        var allClaims = Permissions.All.Select(p => p.Claim).ToList();

        foreach (var permission in permissions)
        {
            if (!allClaims.Contains(permission))
            {
                return Results.BadRequest($"{permission} is not a valid permission");
            }
        }

        permissions.Add(Permissions.View.Claim);

        var claims = await userManager.GetClaimsAsync(admin);
        await userManager.RemoveClaimsAsync(admin, claims);

        foreach (var permission in permissions)
        {
            await userManager.AddClaimAsync(admin, new Claim(permission, ""));
        }

        admin.Permissions.Clear();
        admin.Permissions.AddRange(permissions);

        await db.SaveChangesAsync();

        return Results.Ok<AdminUserDto>(AdminUserDto.Map(admin));
    }
}
