using System.Threading.Tasks;
using Core.Models;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Context.Admins;
using Shared.Infrastructure.Errors;

namespace Core.Features.Admins.Commands;

public static class LoginAdminCommand
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

    public static async Task<Results<Ok, BadRequest<ProblemDetails>, ValidationProblem>> Handle(
        Request request,
        IValidator<Request> validator,
        SignInManager<Admin> signInManager
    )
    {
        if (validator.TryValidate(request, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var result = await signInManager.PasswordSignInAsync(request.Email, request.Password, true, false);

        if (!result.Succeeded)
        {
            return Result.Fail(AdminErrors.LoginError()).ToBadRequestProblem();
        }

        return TypedResults.Ok();
    }
}