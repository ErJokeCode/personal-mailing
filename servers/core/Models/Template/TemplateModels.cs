using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models;

public class NotificationTemplate
{
    public int Id { get; set; }

    public string Content { get; set; }
    public string Date { get; set; }

    public List<int> DocumentIds { get; set; } = [];
    [NotMapped]
    public List<Document> Documents { get; } = [];

    public List<Guid> StudentIds { get; set; } = [];
    [NotMapped]
    public List<ActiveStudent> ActiveStudents { get; } = [];

    public string AdminId { get; set; }
    public AdminUser Admin { get; set; }
}
