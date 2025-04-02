using System;
using System.Collections.Generic;
using Core.Routes.Documents.DTOs;
using Microsoft.EntityFrameworkCore;
using Notify.Consumers.Admins;
using Notify.Consumers.Students;
using Riok.Mapperly.Abstractions;

namespace Core.Routes.Notifications.Dtos;

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