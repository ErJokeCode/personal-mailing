
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions;
using Core.Infrastructure.Services;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Errors;
using Core.Routes.Admins.Maps;
using FluentResults;
using MediatR;

namespace Core.Routes.Admins.Queries;

public class GetAdminMeQuery : IRequest<Result<AdminDto>>;

public class GetAdminMeQueryHandler : IRequestHandler<GetAdminMeQuery, Result<AdminDto>>
{
    private readonly AdminMapper _adminMapper;
    private readonly IUserAccessor _userAccessor;

    public GetAdminMeQueryHandler(IUserAccessor userAccessor)
    {
        _adminMapper = new AdminMapper();
        _userAccessor = userAccessor;
    }

    public async Task<Result<AdminDto>> Handle(GetAdminMeQuery request, CancellationToken cancellationToken)
    {
        var admin = await _userAccessor.GetUserAsync();

        if (admin is null)
        {
            return Result.Fail<AdminDto>(AdminErrors.NotFound());
        }

        return Result.Ok(_adminMapper.Map(admin));
    }
}