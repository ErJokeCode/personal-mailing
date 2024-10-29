using System;
using System.Collections.Generic;

namespace Core.Models.Dto;

public class NotificationDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }
    public List<string> FileNames { get; set; }

    public string AdminId { get; set; }
    public ICollection<Guid> StudentIds { get; } = [];
}
