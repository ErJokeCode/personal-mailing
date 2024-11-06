using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static class ChatHandler
{
    public static async Task<IResult> AdminSendToStudent(string content, Guid studentId, CoreDb db, HttpContext context,
                                                         UserManager<AdminUser> userManager)
    {
        var adminId = userManager.GetUserId(context.User);

        if (adminId == null)
        {
            return Results.NotFound("Admin not found");
        }
        var admin = db.Users.Find(adminId);

        var activeStudent = await db.ActiveStudents.Include(a => a.Chats).SingleOrDefaultAsync(s => s.Id == studentId);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        var chat = activeStudent.Chats.SingleOrDefault(ch => ch.ActiveStudentId == activeStudent.Id);

        if (chat == null)
        {
            chat = new Chat() {
                ActiveStudentId = activeStudent.Id,
                AdminId = adminId,
            };

            db.Chats.Add(chat);
        }

        var message = new Message() {
            Date = DateTime.Now.ToString(),
            Sender = "Admin",
            Receiver = "Student",
            Content = content,
        };

        var status = new MessageStatus();
        status.SetLost();
        message.Status = status;

        chat.Messages.Add(message);

        await db.SaveChangesAsync();

        return Results.Ok(MessageDto.Map(message));
    }

    public static async Task<IResult> GetAdminChats(HttpContext context, UserManager<AdminUser> userManager, CoreDb db)
    {
        var adminId = userManager.GetUserId(context.User);

        if (adminId == null)
        {
            return Results.NotFound("Admin not found");
        }

        var admin = db.Users.Include(a => a.Chats)
                        .ThenInclude(ch => ch.Messages)
                        .ThenInclude(m => m.Status)
                        .Include(a => a.Chats)
                        .ThenInclude(ch => ch.ActiveStudent)
                        .SingleOrDefault(a => a.Id == adminId);

        return Results.Ok(ChatDto.Maps(admin.Chats.ToList()));
    }

    public static async Task<IResult> GetChatById(int id, CoreDb db)
    {
        var chat = db.Chats.Include(ch => ch.Messages)
                       .ThenInclude(m => m.Status)
                       .Include(ch => ch.ActiveStudent)
                       .Include(ch => ch.Admin)
                       .SingleOrDefault(ch => ch.Id == id);

        if (chat == null)
        {
            return Results.NotFound("Chat not found");
        }

        return Results.Ok(ChatDto.Map(chat));
    }

    public static async Task<IResult> GetAdminChatWithStudent(Guid studentId, CoreDb db,
                                                              UserManager<AdminUser> userManager, HttpContext context)
    {
        var adminId = userManager.GetUserId(context.User);

        var chat = db.Chats.Include(ch => ch.Messages)
                       .ThenInclude(m => m.Status)
                       .Include(ch => ch.ActiveStudent)
                       .Include(ch => ch.Admin)
                       .SingleOrDefault(ch => ch.AdminId == adminId && ch.ActiveStudentId == studentId);

        if (chat == null)
        {
            return Results.NotFound("Chat not found");
        }

        return Results.Ok(ChatDto.Map(chat));
    }

    public static async Task<IResult> StudentSendToAdmin(string content, Guid studentId, string adminId, CoreDb db)
    {
        var admin = db.Users.Find(adminId);

        if (adminId == null)
        {
            return Results.NotFound("Admin not found");
        }

        var activeStudent = await db.ActiveStudents.Include(a => a.Chats).SingleOrDefaultAsync(s => s.Id == studentId);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        var chat = activeStudent.Chats.SingleOrDefault(ch => ch.ActiveStudentId == activeStudent.Id);

        if (chat == null)
        {
            chat = new Chat() {
                ActiveStudentId = activeStudent.Id,
                AdminId = adminId,
            };

            db.Chats.Add(chat);
        }

        var message = new Message() {
            Date = DateTime.Now.ToString(),
            Sender = "Student",
            Receiver = "Admin",
            Content = content,
        };

        var status = new MessageStatus();
        status.SetLost();
        message.Status = status;

        chat.Messages.Add(message);

        await db.SaveChangesAsync();

        return Results.Ok();
    }

    public static async Task<IResult> GetStudentChats(Guid id, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.Include(a => a.Chats)
                                .ThenInclude(ch => ch.Messages)
                                .ThenInclude(m => m.Status)
                                .Include(a => a.Chats)
                                .ThenInclude(ch => ch.Admin)
                                .SingleOrDefault(a => a.Id == id);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        return Results.Ok(ChatDto.Maps(activeStudent.Chats.ToList()));
    }

    public static async Task<IResult> SetMessageStatus(int id, int code, CoreDb db)
    {
        var status = await db.MessageStatuses.SingleOrDefaultAsync(n => n.MessageId == id);

        if (status == null)
        {
            return Results.NotFound("Status not found");
        }

        switch (code)
        {
        case -1:
            status.SetLost();
            break;
        case 0:
            status.SetSent();
            break;
        case 1:
            status.SetRead();
            break;
        default:
            return Results.BadRequest("Wrong status code");
        }

        await db.SaveChangesAsync();

        return Results.Ok(MessageStatusDto.Map(status));
    }
}
