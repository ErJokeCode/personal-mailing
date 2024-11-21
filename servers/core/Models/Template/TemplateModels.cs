using System.Collections.Generic;

namespace Core.Models;

public class NotificationTemplate
{
    public int Id { get; set; }

    public string Content { get; set; }
    public string Date { get; set; }

    public string AdminId { get; set; }
    public AdminUser Admin { get; set; }

    public ICollection<Document> Documents { get; } = [];
    public ICollection<ActiveStudent> ActiveStudents { get; } = [];
}
