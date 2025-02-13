using System;
using Core.Abstractions.Parser;

namespace Core.Routes.Students.Dtos;

public class StudentDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string ChatId { get; set; }
    public required DateOnly CreatedAt { get; set; }

    public required ParserStudent Info { get; set; }

    public bool Active { get; set; }
    public DateOnly? DeactivatedAt { get; set; }
}