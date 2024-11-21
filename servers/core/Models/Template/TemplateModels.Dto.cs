using System.Collections.Generic;

namespace Core.Models.Dto;

public partial class NotificationTemplateDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }

    public AdminUserDto Admin { get; set; }

    public List<DocumentDto> Documents { get; } = [];
    public List<ActiveStudentDto> Students { get; } = [];
}
