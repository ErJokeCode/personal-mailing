using System;
using System.Threading.Tasks;
using Core.Features.Admins.Commands;
using Core.Features.Admins.DTOs;
using Core.Models;
using Core.Routes.Admins.Maps;
using FluentResults;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Shared.Context.Admins;
using Shared.Context.Admins.Messages;

public class AdminService
{
    private readonly AdminMapper _adminMapper;
    private readonly ITopicProducer<AdminCreatedMessage> _topicProducer;
    private readonly UserManager<Admin> _userManager;

    public AdminService(AdminMapper adminMapper, ITopicProducer<AdminCreatedMessage> topicProducer, UserManager<Admin> userManager)
    {
        _adminMapper = adminMapper;
        _topicProducer = topicProducer;
        _userManager = userManager;
    }

    public async Task<Result<AdminDto>> CreateAdminAsync(CreateAdminCommand.Request request)
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

        var dto = _adminMapper.Map(newUser);

        await _topicProducer.Produce(_adminMapper.MapToMessage(newUser));

        return Result.Ok(dto);
    }
}