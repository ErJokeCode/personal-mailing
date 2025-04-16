using System.Threading.Tasks;
using Core.Features.Admins.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Errors;

namespace Core.Features.Admins.Commands;

public static class CreateAdminCommand
{
    public class Request
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithName("Почта");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithName("Пароль");
        }
    }

    public static async Task<Results<Ok<AdminDto>, BadRequest<ProblemDetails>, ValidationProblem>> Handle(
        Request request,
        IValidator<Request> validator,
        AdminService adminService
    )
    {
        if (validator.TryValidate(request, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var result = await adminService.CreateAdminAsync(request);

        if (result.IsFailed)
        {
            return result.ToBadRequestProblem();
        }

        return TypedResults.Ok(result.Value);
    }
}