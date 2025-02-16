using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Infrastructure.Search;
using Core.Models;
using Core.Routes.Notifications.Dtos;
using Core.Routes.Notifications.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Notifications.Queries;

public class GetAllNotificationsQuery : IRequest<IEnumerable<NotificationDto>>
{
    public string? Content { get; set; }
    public Guid? AdminId { get; set; }
    public Guid? StudentId { get; set; }
}

public class GetAllNotificationsQueryHandler : IRequestHandler<GetAllNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly AppDbContext _db;
    private readonly NotificationMapper _notificationMapper;

    public GetAllNotificationsQueryHandler(AppDbContext db)
    {
        _db = db;
        _notificationMapper = new NotificationMapper();
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _db.Notifications
            .Include(n => n.Admin)
            .Include(n => n.Students)
            .AsSplitQuery()
            .ToListAsync();

        notifications = FilterNotifications(notifications, request).ToList();

        return _notificationMapper.Map(notifications);
    }

    private IEnumerable<Notification> FilterNotifications(IEnumerable<Notification> notifications, GetAllNotificationsQuery request)
    {
        if (!string.IsNullOrEmpty(request.Content))
        {
            notifications = notifications
                .Where(n => FuzzySearch.Contains(n.Content, request.Content));
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