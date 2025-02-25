using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Infrastructure.Rest;
using Core.Models;
using Core.Routes.Notifications.Dtos;
using Core.Routes.Notifications.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Notifications.Queries;

public class GetAllNotificationsQuery : IRequest<PagedList<NotificationDto>>
{
    public string? Search { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public Guid? AdminId { get; set; }
    public Guid? StudentId { get; set; }
}

public class GetAllNotificationsQueryHandler : IRequestHandler<GetAllNotificationsQuery, PagedList<NotificationDto>>
{
    private readonly AppDbContext _db;
    private readonly NotificationMapper _notificationMapper;

    public GetAllNotificationsQueryHandler(AppDbContext db)
    {
        _db = db;
        _notificationMapper = new NotificationMapper();
    }

    public async Task<PagedList<NotificationDto>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _db.Notifications
            .Include(n => n.Admin)
            .Include(n => n.Students)
            .AsSplitQuery()
            .ToListAsync();

        notifications = FilterNotifications(notifications, request).ToList();

        return PagedList<NotificationDto>.Create(_notificationMapper.Map(notifications), request.Page, request.PageSize);
    }

    private IEnumerable<Notification> FilterNotifications(IEnumerable<Notification> notifications, GetAllNotificationsQuery request)
    {
        if (!string.IsNullOrEmpty(request.Search))
        {
            notifications = notifications
                .Where(n => FuzzySearch.Contains(n.Content, request.Search));
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