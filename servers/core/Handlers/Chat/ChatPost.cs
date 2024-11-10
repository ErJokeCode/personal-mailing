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
        chat.Messages.Add(message);

        var sent = await BotHandler.SendToBot(activeStudent.AdminChatId, details.Content, documents, false);

        if (sent)
        {
            message.Status.SetSent();
        }

        await db.SaveChangesAsync();

        await DocumentHandler.StoreDocuments(documents, message.Id, db, false);

        return Results.Ok(MessageDto.Map(message));
    }

    public class StudentMessage
    {
        public string Content { get; set; }
        public Guid StudentId { get; set; }
        public string AdminId { get; set; }
    }

    public static async Task<IResult> StudentSendToAdmin([FromForm] string body,
                                                         [FromForm] IFormFileCollection documents,
                                                         IPublishEndpoint endpoint, CoreDb db)
    {
        var details = JsonSerializer.Deserialize<StudentMessage>(
            body, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var admin = db.Users.Find(details.AdminId);

        if (details.AdminId == null)
        {
            return Results.NotFound("Admin not found");
        }

        var activeStudent =
            await db.ActiveStudents.Include(a => a.Chats).SingleOrDefaultAsync(s => s.Id == details.StudentId);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        var chat = activeStudent.Chats.SingleOrDefault(ch => ch.ActiveStudentId == activeStudent.Id);

        if (chat == null)
        {
            chat = new Chat()
            {
                ActiveStudentId = activeStudent.Id,
                AdminId = details.AdminId,
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
        chat.Messages.Add(message);

        await db.SaveChangesAsync();

        await DocumentHandler.StoreDocuments(documents, message.Id, db, false);

        await endpoint.Publish(new StudentSentMessage()
        {
            Admin = AdminUserDto.Map(admin),
            Message = MessageDto.Map(message),
            Student = ActiveStudentDto.Map(activeStudent)
        });

        return Results.Ok();
    }
}
