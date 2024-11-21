using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class TemplateHandler
{
    public static async Task<IResult> DeleteTemplate(int id, CoreDb db)
    {
        var template = await db.NotificationTemplates.SingleOrDefaultAsync(t => t.Id == id);

        if (template == null)
        {
            return Results.NotFound("Template not found");
        }

        db.NotificationTemplates.Remove(template);

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
