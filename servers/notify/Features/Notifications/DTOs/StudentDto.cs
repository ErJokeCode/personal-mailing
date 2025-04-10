using System;
using Shared.Models;

namespace Notify.Features.Notifications.DTOs;

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