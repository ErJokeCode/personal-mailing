using MediatR;

namespace Core.Routes.Admins.Commands;

public class RegisterAdminCommand : IRequest<Unit>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}