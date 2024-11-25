using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Core.Models.Dto;

public partial class ChatDto : IMappable<ChatDto, Chat>
{
    public static ChatDto Map(Chat orig)
    {
        var dto = new ChatDto()
        {
            Id = orig.Id,
            Admin = AdminUserDto.Map(orig.Admin),
            Student = ActiveStudentDto.Map(orig.ActiveStudent),
            UnreadCount = orig.UnreadCount,
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

public partial class MessageDto : IMappable<MessageDto, Message>
{
    public static MessageDto Map(Message orig)
    {
        var dto = new MessageDto()
        {
            Id = orig.Id,
            Date = orig.Date,
            Content = orig.Content,
            Sender = orig.Sender,
            Receiver = orig.Receiver,
            Status = MessageStatusDto.Map(orig.Status)
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

public partial class MessageStatusDto : IMappable<MessageStatusDto, MessageStatus>
{
    public static MessageStatusDto Map(MessageStatus orig)
    {
        return new MessageStatusDto()
        {
            Code = orig.Code,
            Short = orig.Short,
            Description = orig.Description,
        };
    }

    public static List<MessageStatusDto> Maps(List<MessageStatus> origs)
    {
        return origs.Select(o => MessageStatusDto.Map(o)).ToList();
    }
}

public static class ChatExtensions
{
    public static void BuildChat(this ModelBuilder builder)
    {
        builder.Entity<Chat>().HasMany(e => e.Messages).WithOne(e => e.Chat).HasForeignKey(e => e.ChatId);

        builder.Entity<Message>().HasOne(e => e.Status).WithOne(e => e.Message).HasForeignKey("MessageStatus");
    }
}
