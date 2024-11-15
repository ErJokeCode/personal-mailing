using System.Linq;
using Core.Identity;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static class DataHandler
{
    public static IResult GetAllPermissions()
    {
        return Results.Ok(Permissions.All.Select(p => p.Claim));
    }
}
