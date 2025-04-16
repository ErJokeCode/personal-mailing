using System;

namespace Core.Features.Admins.DTOs;

public class AdminDto
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required DateOnly CreatedAt { get; set; }
}