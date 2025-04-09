using System;
using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using Shared.Models;

namespace Chatter.Models;

public class Student
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string ChatId { get; set; }
    public required DateOnly CreatedAt { get; set; }

    public required ParserStudent Info { get; set; }

    public bool Active { get; set; } = true;
    public DateOnly? DeactivatedAt { get; set; }
}
