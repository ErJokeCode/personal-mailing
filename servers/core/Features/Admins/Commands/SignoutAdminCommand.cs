using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Core.Features.Admins.Commands;

public class SignoutAdminCommand
{
    public static async Task<Ok> Handle(SignInManager<Admin> signInManager)
    {
        await signInManager.SignOutAsync();

        return TypedResults.Ok();
    }
}