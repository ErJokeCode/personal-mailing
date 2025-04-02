using System;
using Notify.Abstractions.Parser;

namespace Notify.Messages.Students;

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

public class StudentAuthedMessage
{
    public required StudentDto Student { get; set; }
}