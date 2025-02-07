
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
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
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly UserManager<Admin> _userManager;

    public GetAdminMeQueryHandler(AppDbContext db, IHttpContextAccessor contextAccessor, UserManager<Admin> userManager)
    {
        _adminMapper = new AdminMapper();
        _contextAccessor = contextAccessor;
        _userManager = userManager;
    }

    public async Task<Result<AdminDto>> Handle(GetAdminMeQuery request, CancellationToken cancellationToken)
    {
        var principal = _contextAccessor.HttpContext!.User;
        var admin = await _userManager.GetUserAsync(principal);

        if (admin is null)
        {
            return Result.Fail<AdminDto>("Админ не был найден");
        }

        return Result.Ok(_adminMapper.Map(admin));
    }
}