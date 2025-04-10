using System;
using System.Threading;
using System.Threading.Tasks;
using Chatter.Routes.Groups.Maps;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Chatter.Data;
using Chatter.Routes.Notifications.DTOs;
using Shared.Context.Admins;

namespace Chatter.Routes.Admins.Queries;

public class GetAdminByIdQuery : IRequest<Result<AdminDto>>
{
    public required Guid AdminId { get; init; }
}

public class GetAdminByIdQueryHandler : IRequestHandler<GetAdminByIdQuery, Result<AdminDto>>
{
    private readonly AppDbContext _db;
    private readonly GroupMapper _adminMapper;

    public GetAdminByIdQueryHandler(AppDbContext db)
    {
        _db = db;
        _adminMapper = new GroupMapper();
    }

    public async Task<Result<AdminDto>> Handle(GetAdminByIdQuery request, CancellationToken cancellationToken)
    {
        var admin = await _db.Users.SingleOrDefaultAsync(a => a.Id == request.AdminId, cancellationToken);

        if (admin is null)
        {
            return Result.Fail<AdminDto>(AdminErrors.NotFound(request.AdminId));
        }

        return Result.Ok(_adminMapper.Map(admin));
    }
}