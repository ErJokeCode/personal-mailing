using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Models;
using Core.Routes.Notifications.Dtos;
using Core.Routes.Notifications.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Students.Queries;

public class GetStudentNotificationsQuery : IRequest<IEnumerable<NotificationDto>>
{
    public required Guid StudentId { get; set; }
}

public class GetStudentNotificationsQueryHandler : IRequestHandler<GetStudentNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly AppDbContext _db;
    private readonly NotificationMapper _notificationMapper;

    public GetStudentNotificationsQueryHandler(AppDbContext db)
    {
        _db = db;
        _notificationMapper = new NotificationMapper();
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetStudentNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _db.Notifications
            .Include(n => n.Admin)
            .Where(n => n.Students.Any(s => s.Id == request.StudentId))
            .ToListAsync();

        return _notificationMapper.Map(notifications);
    }
}
