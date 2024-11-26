using System.Linq;
using System.Threading.Tasks;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class TemplateHandler
{
    public static IResult GetAllTemplates(CoreDb db, int pageIndex = 0, int pageSize = -1)
    {
        var templates = db.NotificationTemplates.Include(n => n.Admin)
                            .Include(t => t.Documents)
                            .Include(t => t.ActiveStudents)
                            .ToList();

        var dtos = NotificationTemplateDto.Maps(templates.ToList());
        var paginated = PaginatedList.Create(dtos.ToList(), pageIndex, pageSize);

        return Results.Ok(paginated);
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
