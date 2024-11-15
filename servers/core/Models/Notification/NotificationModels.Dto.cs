using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models.Dto;

public partial class NotificationDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }

    public List<DocumentDto> Documents { get; } = [];
    public List<NotificationStatusDto> Statuses { get; } = [];

    public AdminUserDto Admin { get; set; }
    public List<ActiveStudentDto> Students { get; } = [];
}

public partial class NotificationStatusDto
{
    public int Code { get; set; }
    public string Short { get; set; }
    public string Description { get; set; }
    public Guid StudentId { get; set; }
}
