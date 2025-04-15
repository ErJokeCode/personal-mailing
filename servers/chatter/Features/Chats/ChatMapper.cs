using System.Collections.Generic;
using System.Text.Json;
using Chatter.Features.Chats.DTOs;
using Chatter.Models;
using Riok.Mapperly.Abstractions;
using Shared.Infrastructure.Extensions;

namespace Chatter.Features.Chats;

[Mapper]
public partial class ChatMapper : IMapper
{
    public partial ChatDto Map(Models.Chat chat);
    public partial IEnumerable<ChatDto> Map(IEnumerable<Chatter.Models.Chat> chats);

    public partial MessageDto Map(Message message);
    public partial IEnumerable<MessageDto> Map(IEnumerable<Message> messages);

    public SendMessageDto? Map(string body)
    {
        return JsonSerializer.Deserialize<SendMessageDto>(
            body,
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        );
    }
}