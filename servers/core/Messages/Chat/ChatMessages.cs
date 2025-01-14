using Core.Models;
using Core.Models.Dto;

namespace Core.Messages;

public class StudentSentMessage
{
    public MessageDto Message { get; set; }
    public AdminUserDto Admin { get; set; }
    public ActiveStudentDto Student { get; set; }
}

public class ChatRead
{
    public string AdminId { get; set; }
    public int ChatId { get; set; }
}
