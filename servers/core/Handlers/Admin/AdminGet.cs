using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class AdminHandler
{
    public static async Task<IResult> GetAdmin(string id, CoreDb db)
    {
        var admin = await db.Users.FindAsync(id);

        if (admin == null)
        {
            return Results.NotFound("Admin not found");
        }

        var dto = AdminUserDto.Map(admin);

        return Results.Ok(dto);
    }

    public static async Task<IResult> GetAdminMe(HttpContext context, UserManager<AdminUser> userManager, CoreDb db)
    {
        var adminId = userManager.GetUserId(context.User);

        if (adminId == null)
        {
            return Results.NotFound("Admin not found");
        }

        var admin = await db.Users.FindAsync(adminId);

        var dto = AdminUserDto.Map(admin);

        return Results.Ok(dto);
    }

    public static IResult GetAdminByEmail(string email, CoreDb db)
    {
        var admin = db.Users.SingleOrDefault(a => a.Email == email);

        if (admin == null)
        {
            return Results.NotFound("Admin not found");
        }

        return Results.Ok(AdminUserDto.Map(admin));
    }

    public static IResult GetAllAdmins(CoreDb db)
    {
        var dtos = AdminUserDto.Maps(db.Users.ToList());

        return Results.Ok(dtos);
    }

    public static async Task<IResult> GetAdminChats(HttpContext context, UserManager<AdminUser> userManager, CoreDb db)
    {
        var adminId = userManager.GetUserId(context.User);

        if (adminId == null)
        {
            return Results.NotFound("Admin not found");
        }

        var admin = await db.Users.Include(a => a.Chats)
                        .ThenInclude(ch => ch.Messages.OrderByDescending(m => m.Date).Take(1))
                        .ThenInclude(m => m.Status)
                        .Include(a => a.Chats)
                        .ThenInclude(ch => ch.Messages)
                        .ThenInclude(m => m.Documents)
                        .Include(a => a.Chats)
                        .ThenInclude(ch => ch.ActiveStudent)
                        .SingleOrDefaultAsync(a => a.Id == adminId);

        var dtos = ChatDto.Maps(admin.Chats.ToList());

        return Results.Ok(dtos);
    }

    public static async Task<IResult> GetAdminNotifications(HttpContext context, UserManager<AdminUser> userManager,
                                                            CoreDb db)
    {
        var id = userManager.GetUserId(context.User);
        var admin = await db.Users.Include(a => a.Notifications)
                        .ThenInclude(n => n.ActiveStudents)
                        .Include(a => a.Notifications)
                        .ThenInclude(n => n.Documents)
                        .Include(a => a.Notifications)
                        .ThenInclude(n => n.Statuses)
                        .SingleOrDefaultAsync(a => a.Id == id);

        if (admin == null)
        {
            return Results.NotFound("Could not find admin");
        }

        var dtos = NotificationDto.Maps((List<Notification>)admin.Notifications);

        return Results.Ok(dtos);
    }
}
