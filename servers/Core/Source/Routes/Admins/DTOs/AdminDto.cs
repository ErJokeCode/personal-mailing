using System;

namespace Core.Routes.Admins.Dtos;

public class AdminDto
{
    public Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public DateOnly CreatedAt { get; init; }
}