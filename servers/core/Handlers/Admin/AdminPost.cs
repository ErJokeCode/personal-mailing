
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
    }

    public static async Task<IResult> AddAdmin(AdminDetails details, HttpContext context,
                                                UserManager<AdminUser> userManager, IUserStore<AdminUser> userStore,
                                                CoreDb db)
    {
        var created = await CreateAdmin(details.Email, details.Password,
                                        [Permissions.SendNotifications, Permissions.View], userManager, userStore, db);
        if (!created)
        {
            return Results.BadRequest("Could not create an admin");
        }

        return Results.Ok();
    }
}
