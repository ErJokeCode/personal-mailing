using System;
using System.Collections.Generic;
using Core.Models;
using Core.Routes.Notifications.Dtos;

namespace Core.Routes.Admins.Dtos;

public class AdminDto
{
    public Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required DateOnly CreatedAt { get; init; }

    public IEnumerable<NotificationDto> Notifications { get; } = [];
}