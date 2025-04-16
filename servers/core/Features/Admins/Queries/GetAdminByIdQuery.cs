using System;
using System.Threading.Tasks;
using Core.Data;
using Core.Features.Admins.DTOs;
using Core.Routes.Admins.Maps;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Context.Admins;
using Shared.Infrastructure.Errors;

namespace Core.Features.Admins.Queries;

public static class GetAdminByIdQuery
{
    public static async Task<Results<Ok<AdminDto>, NotFound<ProblemDetails>>> Handle(
        Guid adminId,
        AppDbContext db,
        AdminMapper adminMapper
    )
    {
        var admin = await db.Users.SingleOrDefaultAsync(a => a.Id == adminId);

        if (admin is null)
        {
            return Result.Fail<AdminDto>(AdminErrors.NotFound(adminId)).ToNotFoundProblem();
        }

        return TypedResults.Ok(adminMapper.Map(admin));
    }

}