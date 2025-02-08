using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.External.Parser;

namespace Core.Models;

public class Student
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string ChatId { get; set; }
    public DateOnly CreatedAt { get; set; }

    [NotMapped]
    public ParserStudent? Info { get; set; }
}
