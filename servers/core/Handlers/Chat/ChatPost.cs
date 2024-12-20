using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Messages;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class ChatHandler
{
    public class AdminMessage
    {
        public string Content { get; set; }
        public Guid StudentId { get; set; }
    }

    public static async Task<IResult> AdminSendToStudent([FromForm] IFormFileCollection documents,
                                                         [FromForm] string body, CoreDb db, HttpContext context,
                                                         UserManager<AdminUser> userManager)
    {
        var details = JsonSerializer.Deserialize<AdminMessage>(
            body, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var adminId = userManager.GetUserId(context.User);

        if (adminId == null)
        {
            return Results.NotFound("Admin not found");
        }
        var admin = db.Users.Find(adminId);

        var activeStudent =
            await db.ActiveStudents.Include(a => a.Chats).SingleOrDefaultAsync(s => s.Id == details.StudentId);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        var found = await activeStudent.IncludeStudent();

        if (!found)
        {
            return Results.NotFound("Student not found");
        }
        else if (!admin.Groups.Contains(activeStudent.Student.Group.Number))
        {
            return Results.NotFound("Student is not linked to you");
        }

        var chat = activeStudent.Chats.SingleOrDefault(ch => ch.ActiveStudentId == activeStudent.Id);

        if (chat == null)
        {
            chat = new Chat()
            {
                ActiveStudentId = activeStudent.Id,
                AdminId = adminId,
            };

            db.Chats.Add(chat);
        }

        var message = new Message()
        {
            Date = DateTime.Now.ToString(),
            Sender = "Admin",
            Receiver = "Student",
            Content = details.Content,
            Status = new MessageStatus(),
        };

        message.Status.SetLost();

        message.Documents.Clear();
        var docs = await DocumentHandler.StoreDocuments(documents, db);
        foreach (var doc in docs)
        {
            message.Documents.Add(doc);
        }

        chat.Messages.Add(message);

        var sent = await BotHandler.SendToBot(activeStudent.ChatId, details.Content, documents, false);

        if (sent)
        {
            message.Status.SetSent();
        }

        await db.SaveChangesAsync();

        return Results.Ok(MessageDto.Map(message));
    }

    public class StudentMessage
    {
        public string Content { get; set; }
        public Guid StudentId { get; set; }
    }

    public static async Task<IResult> StudentSendToAdmin([FromForm] string body,
                                                         [FromForm] IFormFileCollection documents,
                                                         IPublishEndpoint endpoint, CoreDb db)
    {
        var details = JsonSerializer.Deserialize<StudentMessage>(
            body, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var activeStudent =
            await db.ActiveStudents.Include(a => a.Chats).SingleOrDefaultAsync(s => s.Id == details.StudentId);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        var found = await activeStudent.IncludeStudent();

        if (!found)
        {
            return Results.NotFound("Student not found");
        }

        var admin = db.Users.SingleOrDefault(a => a.Groups.Contains(activeStudent.Student.Group.Number));

        if (admin == null)
        {
            return Results.NotFound("Admin not found");
        }

        var chat = activeStudent.Chats.SingleOrDefault(ch => ch.ActiveStudentId == activeStudent.Id);

        if (chat == null)
        {
            chat = new Chat()
            {
                ActiveStudentId = activeStudent.Id,
                AdminId = admin.Id,
            };

            db.Chats.Add(chat);
        }

        var message = new Message()
        {
            Date = DateTime.Now.ToString(),
            Sender = "Student",
            Receiver = "Admin",
            Content = details.Content,
            Status = new MessageStatus(),
        };

        message.Status.SetSent();

        message.Documents.Clear();
        var docs = await DocumentHandler.StoreDocuments(documents, db);
        foreach (var doc in docs)
        {
            message.Documents.Add(doc);
        }

        chat.UnreadCount += 1;
        chat.Messages.Add(message);

        await db.SaveChangesAsync();

        await endpoint.Publish(new StudentSentMessage()
        {
            Admin = AdminUserDto.Map(admin),
            Message = MessageDto.Map(message),
            Student = ActiveStudentDto.Map(activeStudent)
        });

        return Results.Ok();
    }
}
