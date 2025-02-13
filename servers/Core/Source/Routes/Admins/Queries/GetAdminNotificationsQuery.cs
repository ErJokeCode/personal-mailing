using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Routes.Notifications.Dtos;
using Core.Routes.Notifications.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Admins.Queries;

public class GetAdminNotificatioinsQuery : IRequest<IEnumerable<NotificationDto>>
{
    public required Guid AdminId { get; set; }
}

public class GetAdminNotificatioinsQueryHandler : IRequestHandler<GetAdminNotificatioinsQuery, IEnumerable<NotificationDto>>
{
    private readonly AppDbContext _db;
    private readonly NotificationMapper _notificationMapper;

    public GetAdminNotificatioinsQueryHandler(AppDbContext db)
    {
        _db = db;
        _notificationMapper = new NotificationMapper();
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetAdminNotificatioinsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _db.Notifications
            .Where(n => n.AdminId == request.AdminId)
            .ToListAsync();

        return _notificationMapper.Map(notifications);
    }
}
