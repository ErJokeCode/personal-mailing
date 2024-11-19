using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Core.Handlers.NotificationHandler;

namespace Core.Handlers;

public static partial class TemplateHandler
{
    public static async Task<IResult> UpdateTemplate(int id, [FromForm] string body,
                                                     [FromForm] IFormFileCollection documents, CoreDb db)
    {
        var details = JsonSerializer.Deserialize<NotificationDetails>(
            body, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var template = await db.NotificationTemplates.Include(t => t.Admin).SingleOrDefaultAsync(n => n.Id == id);

        if (template == null)
        {
            return Results.NotFound("Template not found");
        }

        template.IncludeDocuments(db).IncludeStudents(db);

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

        var docs = await DocumentHandler.StoreDocuments(documents, db);
        template.DocumentIds.Clear();
        template.DocumentIds.AddRange(docs);

        template.IncludeDocuments(db).IncludeStudents(db);

        await db.SaveChangesAsync();

        return Results.Ok(NotificationTemplateDto.Map(template));
    }
}
