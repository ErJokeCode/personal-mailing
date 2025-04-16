using System.Threading.Tasks;
using Core.Features.Admins.DTOs;
using Core.Routes.Admins.Maps;
using Core.Services.UserAccessor;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Context.Admins;
using Shared.Infrastructure.Errors;

namespace Core.Features.Admins.Queries;

public static class GetAdminMeQuery
{
    public static async Task<Results<Ok<AdminDto>, NotFound<ProblemDetails>>> Handle(
        IUserAccessor userAccessor,
        AdminMapper adminMapper
    )
    {
        var admin = await userAccessor.GetUserAsync();

        if (admin is null)
        {
            return Result.Fail<AdminDto>(AdminErrors.NotFound()).ToNotFoundProblem();
        }

        return TypedResults.Ok(adminMapper.Map(admin));
    }
}