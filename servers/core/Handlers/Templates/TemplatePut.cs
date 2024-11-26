using System.Text.Json;
using System.Threading.Tasks;
using Core.Identity;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Core.Handlers.NotificationHandler;

namespace Core.Handlers;

public static partial class TemplateHandler
{
    public static async Task<IResult> UpdateTemplate(int id, [FromForm] string body,
                                                     [FromForm] IFormFileCollection documents, CoreDb db,
                                                     HttpContext context, UserManager<AdminUser> userManager)
    {
        var details = JsonSerializer.Deserialize<NotificationDetails>(
            body, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var template = await db.NotificationTemplates.Include(t => t.Documents)
                           .Include(t => t.ActiveStudents)
                           .Include(t => t.Admin)
                           .SingleOrDefaultAsync(n => n.Id == id);

        var adminId = userManager.GetUserId(context.User);

        if (template == null)
        {
            return Results.NotFound("Template not found");
        }

        if (template.AdminId != adminId && !context.User.HasClaim(Permissions.ManipulateStudents.Claim, ""))
        {
            return Results.BadRequest("Template does not belong to you");
        }

        template.Content = details.Content;
        template.ActiveStudents.Clear();

        foreach (var guid in details.StudentIds)
        {
            var activeStudent = db.ActiveStudents.Find(guid);

            if (activeStudent == null)
            {
                continue;
            }

            template.ActiveStudents.Add(activeStudent);
        }

        template.Documents.Clear();
        var docs = await DocumentHandler.StoreDocuments(documents, db);
        foreach (var doc in docs)
        {
            template.Documents.Add(doc);
        }

        await db.SaveChangesAsync();

        return Results.Ok(NotificationTemplateDto.Map(template));
    }
}
