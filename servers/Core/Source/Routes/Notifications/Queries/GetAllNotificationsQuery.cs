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

namespace Core.Routes.Notifications.Queries;

public class GetAllNotificationsQuery : IRequest<IEnumerable<NotificationDto>>;

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
        var notifications = await _db.Notifications.Include(n => n.Admin).ToListAsync();

        return _notificationMapper.Map(notifications);
    }
}