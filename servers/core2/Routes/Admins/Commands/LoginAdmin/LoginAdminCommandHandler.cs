using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Routes.Admins.Commands;

public class LoginAdminCommandHandler : IRequestHandler<LoginAdminCommand, Unit>
{
    private readonly SignInManager<Admin> _signInManager;

    public LoginAdminCommandHandler(SignInManager<Admin> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<Unit> Handle(LoginAdminCommand request, CancellationToken cancellationToken)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, true, false);

        if (!result.Succeeded)
        {
            throw new BadHttpRequestException("Плохие данные");
        }

        return Unit.Value;
    }
}