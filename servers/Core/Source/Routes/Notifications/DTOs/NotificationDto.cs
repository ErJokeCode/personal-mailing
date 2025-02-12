
using System;
using System.Collections.Generic;
using Core.Models;
using Core.Routes.Admins.Dtos;
using Core.Routes.Students.Dtos;
using Microsoft.EntityFrameworkCore;
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

    public AdminDto? Admin { get; set; }
    public IEnumerable<StudentDto> Students { get; set; } = [];

    public IEnumerable<NotificationErrorDto> Errors { get; set; } = [];
}