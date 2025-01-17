using System;
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

    public static IResult GetAllAdmins(CoreDb db, string search = null, int pageIndex = 0, int pageSize = -1)
    {
        var admins = db.Users.ToList();

        if (!string.IsNullOrEmpty(search))
        {
            admins = admins.Where(a => a.Email.ToLower().Contains(search.ToLower())).ToList();
        }

        var dtos = AdminUserDto.Maps(admins);

        var paginated = new PaginatedList<AdminUserDto>(dtos.ToList(), pageIndex, pageSize);

        return Results.Ok(paginated);
    }

    public static async Task<IResult> GetAdminChats(HttpContext context, UserManager<AdminUser> userManager, CoreDb db,
                                                    bool onlyUnread = false, int pageIndex = 0, int pageSize = -1)
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

        var chats = admin.Chats.OrderByDescending(ch => DateTime.Parse(ch.Messages.ElementAt(0).Date)).ToList();

        if (onlyUnread)
        {
            chats = chats.Where(ch => ch.UnreadCount > 0).ToList();
        }

        var dtos = ChatDto.Maps(chats);
        var paginated = new PaginatedList<ChatDto>(dtos.ToList(), pageIndex, pageSize);

        return Results.Ok(paginated);
    }

    public static async Task<IResult> GetAdminNotifications(HttpContext context, UserManager<AdminUser> userManager,
                                                            CoreDb db, int pageIndex = 0, int pageSize = -1)
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
        var paginated = new PaginatedList<NotificationDto>(dtos.ToList(), pageIndex, pageSize);

        return Results.Ok(paginated);
    }

    public static async Task<IResult> GetAdminTemplates(HttpContext context, UserManager<AdminUser> userManager,
                                                        CoreDb db, int pageIndex = 0, int pageSize = -1)
    {
        var id = userManager.GetUserId(context.User);

        var admin = await db.Users.Include(a => a.Templates)
                        .ThenInclude(n => n.ActiveStudents)
                        .Include(a => a.Templates)
                        .ThenInclude(n => n.Documents)
                        .SingleOrDefaultAsync(a => a.Id == id);

        if (admin == null)
        {
            return Results.NotFound("Could not find admin");
        }

        var dtos = NotificationTemplateDto.Maps((List<NotificationTemplate>)admin.Templates);
        var paginated = new PaginatedList<NotificationTemplateDto>(dtos.ToList(), pageIndex, pageSize);

        return Results.Ok(paginated);
    }

    public static async Task<IResult> GetAdminSchedules(HttpContext context, UserManager<AdminUser> userManager,
                                                        CoreDb db, int pageIndex = 0, int pageSize = -1)
    {
        var id = userManager.GetUserId(context.User);

        var admin =
            await db.Users.Include(a => a.Schedules).ThenInclude(s => s.Template).SingleOrDefaultAsync(a => a.Id == id);

        if (admin == null)
        {
            return Results.NotFound("Could not find admin");
        }

        var dtos = NotificationScheduleDto.Maps((List<NotificationSchedule>)admin.Schedules);
        var paginated = new PaginatedList<NotificationScheduleDto>(dtos.ToList(), pageIndex, pageSize);

        return Results.Ok(paginated);
    }
}
