using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class NotificationHandler
{
    public class StatusDetails
    {
        public int Code { get; set; }
        public Guid StudentId { get; set; }
    }

    public static async Task<IResult> SetNotificationStatus(int id, StatusDetails details, CoreDb db)
    {
        var status = await db.NotificationStatuses.SingleOrDefaultAsync(n => n.NotificationId == id &&
                                                                             n.StudentId == details.StudentId);

        if (status == null)
        {
            return Results.NotFound("Status not found");
        }

        var set = status.Set(details.Code);

        if (!set)
        {
            return Results.BadRequest("Wrong status code");
        }

        await db.SaveChangesAsync();

        return Results.Ok(NotificationStatusDto.Map(status));
    }

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

        DocumentHandler.DeleteDocuments(template.Documents.Select(d => d.InternalName).ToList());

        var docs = await DocumentHandler.StoreDocuments(documents, db);
        template.DocumentIds.Clear();
        template.DocumentIds.AddRange(docs);

        await db.SaveChangesAsync();

        return Results.Ok(NotificationTemplateDto.Map(template));
    }
}
