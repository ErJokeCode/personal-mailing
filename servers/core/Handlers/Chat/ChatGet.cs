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

public static partial class ChatHandler
{
    public static async Task<IResult> GetAdminChatWithStudent(Guid studentId, CoreDb db,
                                                              UserManager<AdminUser> userManager, HttpContext context,
                                                              int lastMessageId = -1, int amount = 100)
    {
        var adminId = userManager.GetUserId(context.User);

        var chat = await db.Chats.Include(ch => ch.Messages)
                       .ThenInclude(m => m.Status)
                       .Include(ch => ch.Messages)
                       .ThenInclude(m => m.Documents)
                       .Include(ch => ch.ActiveStudent)
                       .Include(ch => ch.Admin)
                       .SingleOrDefaultAsync(ch => ch.AdminId == adminId && ch.ActiveStudentId == studentId);

        if (chat == null)
        {
            return Results.NotFound("Chat not found");
        }

        var dto = ChatDto.Map(chat);
        var skipAmount = 0;

        if (lastMessageId == -1)
        {
            var lastRead = dto.Messages.FirstOrDefault(m => m.Receiver == "Admin" && m.Status.Code == 0);
            var lastReadIndex = dto.Messages.IndexOf(lastRead);
            lastReadIndex -= amount;
            skipAmount = Math.Max(0, lastReadIndex);
        }
        else
        {
            var message = dto.Messages.FirstOrDefault(m => m.Id == lastMessageId);
            var messageIndex = dto.Messages.IndexOf(message);
            messageIndex -= amount;
            skipAmount = Math.Max(0, messageIndex);
        }

        dto.Messages = dto.Messages.Skip(skipAmount).ToList();

        return Results.Ok(dto);
    }
}
