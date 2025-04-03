using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Notify.Consumers.Admins;
using Notify.Consumers.Students;
using Notify.Routes.Documents.DTOs;
using Riok.Mapperly.Abstractions;

namespace Notify.Routes.Notifications.DTOs;

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