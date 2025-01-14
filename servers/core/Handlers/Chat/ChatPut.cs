using System.Threading.Tasks;
using Core.Identity;
using Core.Messages;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class ChatHandler
{
    public static async Task<IResult> SetMessageStatus(int id, int code, HttpContext context,
                                                       UserManager<AdminUser> userManager, CoreDb db)
    {
        var adminId = userManager.GetUserId(context.User);

        var message =
            await db.Messages.Include(m => m.Status).Include(m => m.Chat).SingleOrDefaultAsync(m => m.Id == id);

        if (message == null)
        {
            return Results.NotFound("Message not found");
        }

        if (message.Sender == "Admin" && message.Chat.AdminId != adminId)
        {
            return Results.BadRequest("Message does not belong to you");
        }

        if (message.Sender == "Student" && !context.User.HasClaim(Permissions.ManipulateStudents.Claim, ""))
        {
            return Results.BadRequest("Unauthorized to change student's message status");
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

    public static async Task<IResult> MakeChatRead(int id, CoreDb db, IPublishEndpoint endpoint)
    {
        var chat = await db.Chats.Include(ch => ch.Messages)
                       .ThenInclude(m => m.Status)
                       .Include(ch => ch.Admin)
                       .SingleOrDefaultAsync(ch => ch.Id == id);

        foreach (var message in chat.Messages)
        {
            message.Status.SetRead();
        }

        chat.UnreadCount = 0;

        await endpoint.Publish(new ChatRead()
        {
            ChatId = chat.Id,
            AdminId = chat.Admin.Id,
        });

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
