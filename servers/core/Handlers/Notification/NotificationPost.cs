using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Core.Handlers;

public static partial class NotificationHandler
{
    public class NotificationDetails
    {
        public List<Guid> StudentIds { get; set; }
        public List<int> DocumentIds { get; set; }

        public string Content { get; set; }
    }

    public static async Task<IResult> SendNotification([FromForm] IFormFileCollection documents, [FromForm] string body,
                                                       CoreDb db, HttpContext context,
                                                       UserManager<AdminUser> userManager)
    {
        var details = JsonSerializer.Deserialize<NotificationDetails>(
            body, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var adminId = userManager.GetUserId(context.User);

        if (adminId == null)
        {
            Results.NotFound("Could not get the admin");
        }

        var notification = new Notification()
        {
            Content = details.Content,
            Date = DateTime.Now.ToString(),
            AdminId = adminId,
        };

        var admin = db.Users.Find(adminId);
        notification.Admin = admin;

        List<Document> serverDocs = [];
        foreach (var id in details.DocumentIds)
        {
            var doc = await db.Documents.FindAsync(id);

            if (doc == null)
            {
                return Results.BadRequest($"Document {id} not found");
            }

            serverDocs.Add(doc);
            notification.Documents.Add(doc);
        }

        var docs = await DocumentHandler.StoreDocuments(documents, db);
        foreach (var doc in docs)
        {
            notification.Documents.Add(doc);
        }

        var fileCollection = new FormFileCollection();
        fileCollection.AddRange(documents);

        foreach (var doc in serverDocs)
        {
            var stream = await DocumentHandler.GetDocumentStream(doc.Id, db);
            var formFile = new FormFile(stream, 0, stream.Length, null, doc.Name);
            fileCollection.Add(formFile);
        }

        foreach (var guid in details.StudentIds)
        {
            var activeStudent = db.ActiveStudents.Find(guid);

            if (activeStudent == null)
            {
                continue;
            }

            notification.ActiveStudents.Add(activeStudent);

            var status = new NotificationStatus()
            {
                StudentId = guid,
            };
            status.SetLost();

            notification.Statuses.Add(status);

            var filled = await NotificationHandler.FillTemplate(notification.Content, activeStudent);

            var sent = await BotHandler.SendToBot(activeStudent.ChatId, filled, fileCollection);

            if (sent)
            {
                status.SetSent();
            }
        }

        db.Notifications.Add(notification);
        await db.SaveChangesAsync();

        foreach (var stream in fileCollection)
        {
            stream.OpenReadStream().Close();
            stream.OpenReadStream().Dispose();
        }

        var dto = NotificationDto.Map(notification);
        return Results.Ok(dto);
    }
}
