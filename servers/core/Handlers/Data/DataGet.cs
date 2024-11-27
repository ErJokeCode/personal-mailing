
using System.Linq;
using Core.Identity;
using Core.Utility;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static partial class DataHandler
{
    public static IResult GetAllPermissions()
    {
        return Results.Ok(Permissions.All.Select(p => p.Claim));
    }

    public static IResult GetAllTemplates()
    {
        return Results.Ok(NotificationHandler.Passes.Keys);
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
}
