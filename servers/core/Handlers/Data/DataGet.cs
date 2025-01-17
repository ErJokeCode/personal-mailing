
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public static async Task<IResult> GetGroups(CoreDb db, string search = null)
    {
        var allGroups = await ParserHandler.GetFromParser<List<string>>("/student/number_groups", []);

        var mainAdmin = db.Users.ToList().MinBy(a => DateTime.Parse(a.Date));
        var admins = db.Users.ToList();
        var dict = new Dictionary<string, AdminUserDto>();

        foreach (var admin in admins)
        {
            foreach (var group in admin.Groups)
            {
                dict.Add(group, AdminUserDto.Map(admin));
            }
        }

        foreach (var group in allGroups)
        {
            if (!dict.ContainsKey(group))
            {
                dict.Add(group, AdminUserDto.Map(mainAdmin));
            }
        }

        if (!string.IsNullOrEmpty(search))
        {
            dict = dict.Where(p => p.Key.ToLower().Contains(search.ToLower())).ToDictionary();
        }

        return Results.Ok(dict.OrderBy(pair => pair.Key).ToDictionary());
    }
}
