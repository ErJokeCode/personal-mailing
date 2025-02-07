using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Routes.Admins.Commands;

public class LoginAdminCommand : IRequest<Result>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}