using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.External.Parser;

namespace Core.Routes.Students.Dtos;

public class StudentDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string ChatId { get; set; }
    public DateOnly CreatedAt { get; set; }

    public ParserStudent? Info { get; set; }
}