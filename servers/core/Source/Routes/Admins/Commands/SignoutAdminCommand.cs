using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Routes.Admins.Commands;

public class SignoutAdminCommand : IRequest<Unit>;

public class SignoutAdminCommandHandler : IRequestHandler<SignoutAdminCommand, Unit>
{
    private readonly SignInManager<Admin> _signInManager;

    public SignoutAdminCommandHandler(SignInManager<Admin> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<Unit> Handle(SignoutAdminCommand request, CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();
        return Unit.Value;
    }
}