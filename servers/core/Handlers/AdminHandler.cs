using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Http;
using MassTransit;
using Core.Messages;
using Core.Utility;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Security.Claims;

namespace Core.Handlers;

public static class AdminHandler
{
    public class AdminDetails
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public static async Task CreateAdmin(string email, string password, UserManager<AdminUser> userManager,
                                         IUserStore<AdminUser> userStore)
    {
        var emailStore = (IUserEmailStore<AdminUser>)userStore;
        var user = new AdminUser();

        await userStore.SetUserNameAsync(user, email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, password);
        await userManager.AddClaimAsync(user, new Claim("Admin", ""));
    }

    public static async Task<IResult> AddNewAdmin(AdminDetails details, HttpContext context,
                                                  UserManager<AdminUser> userManager, IUserStore<AdminUser> userStore,
                                                  CoreDb db)
    {
        await CreateAdmin(details.Email, details.Password, userManager, userStore);
        await db.SaveChangesAsync();
        return Results.Ok();
    }

    public static async Task<IResult> GetAdmin(string id, CoreDb db)
    {
        var admin = db.Users.Find(id);

        if (admin == null)
        {
            return Results.NotFound("Admin not found");
        }

        var dto = Mapper.Map(admin);

        return Results.Ok(dto);
    }

    public static async Task<IResult> GetAdminMe(HttpContext context, UserManager<AdminUser> userManager, CoreDb db)
    {
        var adminId = userManager.GetUserId(context.User);

        if (adminId == null)
        {
            return Results.NotFound("Admin not found");
        }

        var admin = db.Users.Find(adminId);

        var dto = Mapper.Map(admin);

        return Results.Ok(dto);
    }

    public static async Task<IResult> GetAllAdmins(CoreDb db)
    {
        var dtos = Mapper.Map(db.Users.ToList());

        return Results.Ok(dtos);
    }
}
