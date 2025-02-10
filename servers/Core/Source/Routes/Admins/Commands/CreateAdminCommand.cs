using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Maps;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Routes.Admins.Commands;

public class CreateAdminCommand : IRequest<Result<AdminDto>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, Result<AdminDto>>
{
    private readonly UserManager<Admin> _userManager;
    private readonly AdminMapper _mapper;

    public CreateAdminCommandHandler(UserManager<Admin> userManager)
    {
        _userManager = userManager;
        _mapper = new AdminMapper();
    }

    public async Task<Result<AdminDto>> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Email);

        if (user != null)
        {
            return Result.Fail("Админ с такой почтой уже существует");
        }

        var newUser = new Admin()
        {
            UserName = request.Email,
            Email = request.Email,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Fail($"Произошла ошибка при регистрации: {errors}");
        }

        return Result.Ok(_mapper.Map(newUser));
    }
}