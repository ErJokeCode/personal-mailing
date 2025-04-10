using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Maps;
using FluentResults;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Context.Admins;
using Shared.Context.Admins.Messages;

namespace Core.Routes.Admins.Commands;

public class CreateAdminCommand : IRequest<Result<Dtos.AdminDto>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, Result<Dtos.AdminDto>>
{
    private readonly UserManager<Admin> _userManager;
    private readonly AdminMapper _mapper;
    private readonly ITopicProducer<AdminCreatedMessage> _topicProducer;

    public CreateAdminCommandHandler(UserManager<Admin> userManager, ITopicProducer<AdminCreatedMessage> topicProducer)
    {
        _userManager = userManager;
        _mapper = new AdminMapper();
        _topicProducer = topicProducer;
    }

    public async Task<Result<Dtos.AdminDto>> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Email);

        if (user != null)
        {
            return Result.Fail(AdminErrors.AlreadyExists(request.Email));
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
            return Result.Fail(AdminErrors.RegisterError(result.Errors));
        }

        var dto = _mapper.Map(newUser);

        await _topicProducer.Produce(new AdminCreatedMessage()
        {
            Admin = _mapper.MapToMessage(newUser)
        });

        return Result.Ok(dto);
    }
}