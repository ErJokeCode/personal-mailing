using System.Linq;
using System.Threading.Tasks;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class TemplateHandler
{
    public static IResult GetAllTemplates(CoreDb db)
    {
        var templates = db.NotificationTemplates.Include(n => n.Admin)
                            .Include(t => t.Documents)
                            .Include(t => t.ActiveStudents)
                            .ToList();

        return Results.Ok(NotificationTemplateDto.Maps(templates.ToList()));
    }

    public static async Task<IResult> GetTemplate(int id, CoreDb db)
    {
        var template = await db.NotificationTemplates.Include(n => n.Admin)
                           .Include(t => t.Documents)
                           .Include(t => t.ActiveStudents)
                           .SingleOrDefaultAsync(n => n.Id == id);

        if (template == null)
        {
            return Results.NotFound("Template not found");
        }

        return Results.Ok(NotificationTemplateDto.Map(template));
    }
}
