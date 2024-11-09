using System.Threading.Tasks;
using Core.Models;
using Core.Utility;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Security.Claims;
using System.Collections.Generic;
using System;

namespace Core.Handlers;

public static partial class AdminHandler
{
    public static async Task<bool> CreateAdmin(string email, string password, List<string> permissons,
                                               UserManager<AdminUser> userManager, IUserStore<AdminUser> userStore,
                                               CoreDb db)
    {
        var admin = await userManager.FindByEmailAsync(email);

        if (admin != null)
        {
            return true;
        }

        var emailStore = (IUserEmailStore<AdminUser>)userStore;
        var user = new AdminUser() { Date = DateTime.Now.ToString() };

        await userStore.SetUserNameAsync(user, email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            Console.WriteLine("Could not create admin: ");
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error.Description);
            }
            return false;
        }

        foreach (var permisson in permissons)
        {
            await userManager.AddClaimAsync(user, new Claim($"{permisson}", ""));
        }

        return true;
    }
}
