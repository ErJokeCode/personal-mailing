using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Notify.Data;
using Notify.Features.Notifications.DTOs;
using Notify.Models;
using Notify.Routes.Notifications.Maps;
using Shared.Infrastructure.Rest;

namespace Notify.Features.Notifications.Queries;

public static class GetAllNotificationsQuery
{
    public class Request
    {
        public string? Search { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        public Guid? AdminId { get; set; }
        public Guid? StudentId { get; set; }
    }

    public static async Task<PagedList<NotificationDto>> Handle([AsParameters] Request request, AppDbContext db, NotificationMapper notificationMapper)
    {
        var notificationsQuery = db.Notifications
            .Include(n => n.Admin)
            .Include(n => n.Students)
            .AsSplitQuery();

        var notifications = await FilterNotifications(notificationsQuery, request).ToListAsync();

        return PagedList<NotificationDto>.Create(notificationMapper.Map(notifications), request.Page, request.PageSize);
    }

    private static IQueryable<Notification> FilterNotifications(IQueryable<Notification> notifications, Request request)
    {
        if (!string.IsNullOrEmpty(request.Search))
        {
            notifications = notifications
                .Where(n => n.Content.ToLower().Contains(request.Search.ToLower()));
        }

        if (request.AdminId is not null)
        {
            notifications = notifications
                .Where(n => n.AdminId == request.AdminId);
        }

        if (request.StudentId is not null)
        {
            notifications = notifications
                .Where(n => n.Students.Any(s => s.Id == request.StudentId));
        }

        return notifications;
    }
}