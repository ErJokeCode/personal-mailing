using System.Collections.Generic;
using Core.Models;
using Core.Routes.Chats.DTOs;
using Riok.Mapperly.Abstractions;

namespace Core.Routes.Chats.Maps;

[Mapper]
public partial class ChatMapper
{
    public partial ChatDto Map(Chat chat);
    public partial IEnumerable<ChatDto> Map(IEnumerable<Chat> chats);

    public partial MessageDto Map(Message message);
    public partial IEnumerable<MessageDto> Map(IEnumerable<Message> messages);
}