using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class ChatHandler
{
    public static async Task<IResult> SetMessageStatus(int id, int code, CoreDb db)
    {
        var message =
            await db.Messages.Include(m => m.Status).Include(m => m.Chat).SingleOrDefaultAsync(m => m.Id == id);

        if (message == null)
        {
            return Results.NotFound("Message not found");
        }

        var set = message.Status.Set(code);

        if (!set)
        {
            return Results.BadRequest("Wrong status code");
        }

        if (message.Receiver == "Admin" && message.Status.Code == 1)
        {
            message.Chat.UnreadCount -= 1;
        }

        await db.SaveChangesAsync();

        return Results.Ok(MessageDto.Map(message));
    }
}
