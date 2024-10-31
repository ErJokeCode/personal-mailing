using System;
using System.Collections.Generic;

namespace Core.Models.Dto;

public class AdminUserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
}

public class ActiveStudentDto
{
    public Guid Id { get; set; }
    public Student Info { get; set; }
    public string Email { get; set; }
    public string ChatId { get; set; }
}

public class NotificationDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }

    public AdminUserDto Admin { get; set; }
    public List<ActiveStudentDto> Students { get; } = [];

    public List<DocumentDto> Documents { get; } = [];
}

public class DocumentDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string MimeType { get; set; }
    public int NotificationId { get; set; }
}
