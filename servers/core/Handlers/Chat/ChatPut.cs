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
        var status = await db.MessageStatuses.SingleOrDefaultAsync(n => n.MessageId == id);

        if (status == null)
        {
            return Results.NotFound("Status not found");
        }

        var set = status.Set(code);

        if (!set)
        {
            return Results.BadRequest("Wrong status code");
        }

        await db.SaveChangesAsync();

        return Results.Ok(MessageStatusDto.Map(status));
    }
}
