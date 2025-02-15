using System;
using System.Collections.Generic;
using Core.Models;
using Core.Routes.Notifications.Dtos;

namespace Core.Routes.Admins.Dtos;

public class GroupAssignmentDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public AdminDto? Admin { get; set; }
}

public class AdminDto
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required DateOnly CreatedAt { get; set; }
}