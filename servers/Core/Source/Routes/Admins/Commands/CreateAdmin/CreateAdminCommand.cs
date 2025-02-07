using Core.Routes.Admins.Dtos;
using FluentResults;
using MediatR;

namespace Core.Routes.Admins.Commands;

public class CreateAdminCommand : IRequest<Result<AdminDto>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}