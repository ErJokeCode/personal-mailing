using System;

namespace Core.Routes.Students.Dtos;

public class StudentDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string ChatId { get; set; }
    public DateOnly CreatedAt { get; set; }
}