using System;
using System.Collections.Generic;

namespace Core.Models.Dto;

public class NotificationDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }

    public string AdminId { get; set; }
    public List<Guid> StudentIds { get; } = [];

    public List<DocumentDto> Documents { get; } = [];
}

public class DocumentDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string MimeType { get; set; }
    public int NotificationId { get; set; }
}
