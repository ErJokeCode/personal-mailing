using System;
using System.Collections.Generic;
using Core.Models;
using Core.Routes.Notifications.Dtos;

namespace Core.Routes.Admins.Dtos;

public class AdminDto
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required DateOnly CreatedAt { get; set; }
    public required IEnumerable<string> Groups { get; set; } = [];
}