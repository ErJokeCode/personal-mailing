using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class ScheduleHandler
{
    public static async Task SendFromTemplate(int templateId, CoreDb db)
    {
        var template = await db.NotificationTemplates.Include(t => t.Admin)
                           .Include(t => t.Documents)
                           .Include(t => t.ActiveStudents)
                           .SingleOrDefaultAsync(t => t.Id == templateId);

        var notification = new Notification()
        {
            Content = template.Content,
            Date = DateTime.Now.ToString(),
            AdminId = template.AdminId,
            Admin = template.Admin,
        };

        foreach (var student in template.ActiveStudents)
        {
            notification.ActiveStudents.Add(student);
        }
        foreach (var documentId in template.Documents)
        {
            notification.Documents.Add(documentId);
        }

        var fileCollection = new FormFileCollection();

        foreach (var document in template.Documents)
        {
            var stream = await DocumentHandler.GetDocumentStream(document.Id, db);
            var formFile = new FormFile(stream, 0, stream.Length, null, document.Name);
            fileCollection.Add(formFile);
        }

        db.Notifications.Add(notification);

        foreach (var student in template.ActiveStudents)
        {
            await BotHandler.SendToBot(student.ChatId, notification.Content, fileCollection);
        }

        foreach (var stream in fileCollection)
        {
            stream.OpenReadStream().Close();
            stream.OpenReadStream().Dispose();
        }
    }
}
