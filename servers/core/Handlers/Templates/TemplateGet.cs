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
    public static IResult GetAllTemplates(CoreDb db)
    {
        var templates = db.NotificationTemplates.Include(n => n.Admin).ToList();

        templates.IncludeDocuments(db).IncludeStudents(db);

        return Results.Ok(NotificationTemplateDto.Maps(templates.ToList()));
    }

    public static async Task<IResult> GetTemplate(int id, CoreDb db)
    {
        var template = await db.NotificationTemplates.Include(n => n.Admin).SingleOrDefaultAsync(n => n.Id == id);

        if (template == null)
        {
            return Results.NotFound("Template not found");
        }

        template.IncludeDocuments(db).IncludeStudents(db);

        return Results.Ok(NotificationTemplateDto.Map(template));
    }
}
