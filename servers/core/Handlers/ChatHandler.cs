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

        var activeStudent = await db.ActiveStudents.Include(a => a.Chats).SingleAsync(s => s.Id == studentId);

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
        }

        var message = new Message()
        {
            Date = DateTime.Now.ToString(),
            Sender = "Admin",
            Receiver = "Student",
            Content = content,
        };

        chat.Messages.Add(message);
        db.Chats.Add(chat);

        await db.SaveChangesAsync();

        return Results.Ok();
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
                        .Include(a => a.Chats)
                        .ThenInclude(ch => ch.ActiveStudent)
                        .Single(a => a.Id == adminId);

        return Results.Ok(ChatDto.Maps(admin.Chats.ToList()));
    }
}
