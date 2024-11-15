using System.Collections.Generic;
using System.Linq;

namespace Core.Models.Dto;

public partial class ChatDto
{
    public int Id { get; set; }
    public int UnreadCount { get; set; }

    public AdminUserDto Admin { get; set; }
    public ActiveStudentDto Student { get; set; }

    public List<MessageDto> Messages { get; set; } = [];
}

public partial class MessageDto
{
    public int Id { get; set; }

    public string Date { get; set; }
    public string Content { get; set; }

    public string Sender { get; set; }
    public string Receiver { get; set; }

    public MessageStatusDto Status { get; set; }

    public List<DocumentDto> Documents { get; set; } = [];
}

public partial class MessageStatusDto
{
    public int Code { get; set; }
    public string Short { get; set; }
    public string Description { get; set; }
}
