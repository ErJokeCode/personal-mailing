using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Routes.Admins.Commands;

public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, Unit>
{
    private readonly UserManager<Admin> _userManager;

    public RegisterAdminCommandHandler(UserManager<Admin> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Email);

        if (user != null)
        {
            throw new BadHttpRequestException("Админ с такой почтой уже существует");
        }

        var newUser = new Admin()
        {
            UserName = request.Email,
            Email = request.Email,
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadHttpRequestException($"Произошла ошибка при регистрации: {errors}");
        }

        return Unit.Value;
    }
}