using System;
using System.Collections.Generic;

namespace Notify.Features.Notifications.DTOs;

public class NotificationErrorDto
{
    public Guid StudentId { get; set; }
    public required string Message { get; set; }
}

public class NotificationDto
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }

    public IEnumerable<DocumentDto> Documents { get; set; } = [];

    public AdminDto? Admin { get; set; }
    public IEnumerable<StudentDto> Students { get; set; } = [];

    public IEnumerable<NotificationErrorDto> Errors { get; set; } = [];
}