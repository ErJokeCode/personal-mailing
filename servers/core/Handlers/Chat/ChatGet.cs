using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class ChatHandler
{
    public static async Task<IResult> GetAdminChatWithStudent(Guid studentId, CoreDb db,
                                                              UserManager<AdminUser> userManager, HttpContext context)
    {
        var adminId = userManager.GetUserId(context.User);

        var chat = await db.Chats.Include(ch => ch.Messages)
                       .ThenInclude(m => m.Status)
                       .Include(ch => ch.ActiveStudent)
                       .Include(ch => ch.Admin)
                       .SingleOrDefaultAsync(ch => ch.AdminId == adminId && ch.ActiveStudentId == studentId);

        chat.Messages.IncludeDocuments(db);

        if (chat == null)
        {
            return Results.NotFound("Chat not found");
        }

        return Results.Ok(ChatDto.Map(chat));
    }
}
