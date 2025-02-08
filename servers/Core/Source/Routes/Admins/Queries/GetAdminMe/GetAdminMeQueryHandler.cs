
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Infrastructure.Services;
using Core.Models;
using Core.Routes.Admins.Dtos;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Routes.Admins.Queries;

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
            return Result.Fail<AdminDto>("Админ не был найден");
        }

        return Result.Ok(_adminMapper.Map(admin));
    }
}