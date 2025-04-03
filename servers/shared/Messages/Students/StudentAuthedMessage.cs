using System;
using Shared.Models;

namespace Shared.Messages.Students;

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
    public static string TopicName => "student-authed";

    public required StudentDto Student { get; set; }
}