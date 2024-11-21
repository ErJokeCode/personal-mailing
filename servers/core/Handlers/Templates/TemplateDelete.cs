using System.Linq;
using System.Threading.Tasks;
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

        var schedules = await db.NotificationSchedules.Where(s => s.TemplateId == id).ToListAsync();

        if (schedules.Any())
        {
            return Results.NotFound("There are schedules associated with this template");
        }

        template.ActiveStudents.Clear();
        db.NotificationTemplates.Remove(template);

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
