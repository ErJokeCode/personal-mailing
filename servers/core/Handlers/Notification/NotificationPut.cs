using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class NotificationHandler
{
    public class StatusDetails
    {
        public int Code { get; set; }
        public Guid StudentId { get; set; }
    }

    public static async Task<IResult> SetNotificationStatus(int id, StatusDetails details, CoreDb db)
    {
        var status = await db.NotificationStatuses.SingleOrDefaultAsync(n => n.NotificationId == id &&
                                                                             n.StudentId == details.StudentId);

        if (status == null)
        {
            return Results.NotFound("Status not found");
        }

        var set = status.Set(details.Code);

        if (!set)
        {
            return Results.BadRequest("Wrong status code");
        }

        await db.SaveChangesAsync();

        return Results.Ok(NotificationStatusDto.Map(status));
    }
}
