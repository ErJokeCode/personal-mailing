using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Routes.Admins.Commands;

public class LoginAdminCommand : IRequest<Unit>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}