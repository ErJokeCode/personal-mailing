using System;

namespace Core.Models;

public class Student
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string ChatId { get; set; }
    public DateOnly CreatedAt { get; set; }
}
