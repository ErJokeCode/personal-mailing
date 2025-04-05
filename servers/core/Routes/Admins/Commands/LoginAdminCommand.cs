using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Infrastructure.Errors.Admins;

namespace Core.Routes.Admins.Commands;

public class LoginAdminCommand : IRequest<Result>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginAdminCommandHandler : IRequestHandler<LoginAdminCommand, Result>
{
    private readonly SignInManager<Admin> _signInManager;

    public LoginAdminCommandHandler(SignInManager<Admin> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<Result> Handle(LoginAdminCommand request, CancellationToken cancellationToken)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, true, false);

        if (!result.Succeeded)
        {
            return Result.Fail(AdminErrors.LoginError());
        }

        return Result.Ok();
    }
}