using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Core.External.Parser;
using Core.Models;
using Core.Routes.Notifications.Dtos;

namespace Core.Routes.Students.Dtos;

public class StudentDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string ChatId { get; set; }
    public required DateOnly CreatedAt { get; set; }

    public ParserStudent? Info { get; set; }

    public IEnumerable<NotificationDto> Notifications { get; } = [];
}