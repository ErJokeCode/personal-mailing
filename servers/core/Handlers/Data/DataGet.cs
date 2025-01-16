
using System.Collections.Generic;
using System.Linq;
using Core.Identity;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static partial class DataHandler
{
    public static IResult GetAllPermissions()
    {
        return Results.Ok(Permissions.All);
    }

    public static IResult GetText(CoreDb db)
    {
        var text = db.Texts.OrderBy(t => t.Id).LastOrDefault();

        if (text == null)
        {
            return Results.NotFound("Text not found");
        }

        return Results.Ok(text);
    }

    public static IResult GetGroups(CoreDb db, string search = null)
    {
        var admins = db.Users.ToList();
        var dict = new Dictionary<string, AdminUserDto>();

        foreach (var admin in admins)
        {
            foreach (var group in admin.Groups)
            {
                dict.Add(group, AdminUserDto.Map(admin));
            }
        }

        if (!string.IsNullOrEmpty(search))
        {
            dict = dict.Where(p => p.Key.ToLower().Contains(search.ToLower())).ToDictionary();
        }

        return Results.Ok(dict);
    }
}
