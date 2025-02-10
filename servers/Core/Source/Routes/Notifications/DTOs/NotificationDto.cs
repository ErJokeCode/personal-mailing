
using System;
using System.Collections.Generic;
using Core.Models;
using Core.Routes.Admins.Dtos;
using Core.Routes.Students.Dtos;

namespace Core.Routes.Notifications.Dtos;

public class NotificationDto
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }

    public AdminDto? Admin { get; set; }
    public IEnumerable<StudentDto> Students { get; set; } = [];
}