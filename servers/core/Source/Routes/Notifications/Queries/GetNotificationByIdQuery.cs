using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Routes.Notifications.Dtos;
using Core.Routes.Notifications.Errors;
using Core.Routes.Notifications.Maps;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Notifications.Queries;

public class GetNotificationByIdQuery : IRequest<Result<NotificationDto>>
{
    public required int NotificationId { get; set; }
}

public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, Result<NotificationDto>>
{
    private readonly AppDbContext _db;
    private readonly NotificationMapper _notificationMapper;

    public GetNotificationByIdQueryHandler(AppDbContext db)
    {
        _db = db;
        _notificationMapper = new NotificationMapper();
    }

    public async Task<Result<NotificationDto>> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var notification = await _db.Notifications
            .Include(n => n.Admin)
            .Include(n => n.Students)
            .AsSplitQuery()
            .SingleOrDefaultAsync(n => n.Id == request.NotificationId);

        if (notification is null)
        {
            return Result.Fail<NotificationDto>(NotificationErrors.NotFound(request.NotificationId));
        }

        return Result.Ok(_notificationMapper.Map(notification));
    }
}
