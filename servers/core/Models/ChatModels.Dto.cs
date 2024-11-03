using System.Collections.Generic;
using System.Linq;

namespace Core.Models.Dto;

public class ChatDto : IMappable<ChatDto, Chat>
{
    public int Id { get; set; }

    public AdminUserDto Admin { get; set; }
    public ActiveStudentDto Student { get; set; }

    public List<MessageDto> Messages { get; set; } = [];

    public static ChatDto Map(Chat orig)
    {
        var dto = new ChatDto() {
            Id = orig.Id,
            Admin = AdminUserDto.Map(orig.Admin),
            Student = ActiveStudentDto.Map(orig.ActiveStudent),
        };

        foreach (var message in orig.Messages)
        {
            dto.Messages.Add(MessageDto.Map(message));
        }

        return dto;
    }

    public static List<ChatDto> Maps(List<Chat> origs)
    {
        return origs.Select(o => ChatDto.Map(o)).ToList();
    }
}

public class MessageDto : IMappable<MessageDto, Message>
{
    public int Id { get; set; }

    public string Date { get; set; }
    public string Content { get; set; }

    public string Sender { get; set; }
    public string Receiver { get; set; }

    public List<DocumentDto> Documents { get; set; } = [];

    public static MessageDto Map(Message orig)
    {
        var dto = new MessageDto() {
            Id = orig.Id, Date = orig.Date, Content = orig.Content, Sender = orig.Sender, Receiver = orig.Receiver,
        };

        foreach (var document in orig.Documents)
        {
            dto.Documents.Add(DocumentDto.Map(document));
        }

        return dto;
    }

    public static List<MessageDto> Maps(List<Message> origs)
    {
        return origs.Select(o => MessageDto.Map(o)).ToList();
    }
}
