using System.Linq;
using System.Threading.Tasks;
using Core.Identity;
using Core.Models;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class TemplateHandler
{
    public static async Task<IResult> DeleteTemplate(int id, CoreDb db, HttpContext context,
                                                     UserManager<AdminUser> userManager)
    {
        var template = await db.NotificationTemplates.SingleOrDefaultAsync(t => t.Id == id);

        if (template == null)
        {
            return Results.NotFound("Template not found");
        }

        var adminId = userManager.GetUserId(context.User);

        if (template.AdminId != adminId && !context.User.HasClaim(Permissions.ManipulateStudents.Claim, ""))
        {
            return Results.BadRequest("Template does not belong to you");
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
