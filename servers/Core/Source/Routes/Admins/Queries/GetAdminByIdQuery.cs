
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Maps;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Admins.Queries;

public class GetAdminByIdQuery : IRequest<Result<AdminDto>>
{
    public required Guid AdminId { get; init; }
}

public class GetAdminByIdQueryHandler : IRequestHandler<GetAdminByIdQuery, Result<AdminDto>>
{
    private readonly AppDbContext _db;
    private readonly AdminMapper _adminMapper;

    public GetAdminByIdQueryHandler(AppDbContext db)
    {
        _db = db;
        _adminMapper = new AdminMapper();
    }

    public async Task<Result<AdminDto>> Handle(GetAdminByIdQuery request, CancellationToken cancellationToken)
    {
        var admin = await _db.Users.SingleOrDefaultAsync(a => a.Id == request.AdminId, cancellationToken);

        if (admin is null)
        {
            return Result.Fail<AdminDto>($"Админ с айди {request.AdminId} не был найден");
        }

        return Result.Ok(_adminMapper.Map(admin));
    }
}