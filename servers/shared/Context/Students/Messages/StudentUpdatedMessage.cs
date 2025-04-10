using System;
using Shared.Models;

namespace Shared.Context.Students.Messages;

public class StudentUpdatedMessage : IMessage
{
    public static string TopicName => "student-updated";

    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string ChatId { get; set; }
    public required DateOnly CreatedAt { get; set; }

    public required ParserStudent Info { get; set; }

    public bool Active { get; set; }
    public DateOnly? DeactivatedAt { get; set; }
}