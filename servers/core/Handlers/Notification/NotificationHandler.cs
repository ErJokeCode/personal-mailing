using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class NotificationHandler
{
    public static Dictionary<string, Func<string, ActiveStudent, string>> Passes =
        new() { { "$name", (content, student) =>
                           { return content.Replace("$name", student.Student.Name); } },

                { "$email", (content, student) =>
                            { return content.Replace("$email", student.Email); } }

        };

    public static async Task<string> FillTemplate(string content, ActiveStudent student)
    {
        if (student.Student == null)
        {
            await student.IncludeStudent();
        }

        foreach (var pass in Passes)
        {
            content = pass.Value(content, student);
        }

        return content;
    }

    public static bool IsFileReady(string filename)
    {
        try
        {
            using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read,
                                                      FileShare.None)) return inputStream.Length > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static async Task SendFromTemplate(int templateId, CoreDb db)
    {
        var template =
            await db.NotificationTemplates.Include(t => t.Admin).SingleOrDefaultAsync(t => t.Id == templateId);

        template.IncludeDocuments(db).IncludeStudents(db);

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
        foreach (var documentId in template.DocumentIds)
        {
            notification.DocumentIds.Add(documentId);
        }

        var fileCollection = new FormFileCollection();

        foreach (var document in template.Documents)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Documents", document.InternalName);

            // This will lock the execution until the file is ready
            // TODO: Add some logic to make it async and cancelable
            while (!IsFileReady(path))
            {
            }

            var stream = File.OpenRead(path);
            var formFile = new FormFile(stream, 0, stream.Length, null, document.Name);
            fileCollection.Add(formFile);
        }

        db.Notifications.Add(notification);

        foreach (var student in template.ActiveStudents)
        {
            await BotHandler.SendToBot(student.ChatId, notification.Content, fileCollection);
        }
    }
}
