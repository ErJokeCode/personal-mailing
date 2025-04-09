using System.Collections.Generic;
using System.Text.Json;
using Chatter.Models;
using Chatter.Routes.Chats.DTOs;
using Chatter.Models;
using Chatter.Routes.Notifications.DTOs;
using Riok.Mapperly.Abstractions;

namespace Chatter.Routes.Chats.Maps;

[Mapper]
public partial class ChatMapper
{
    [MapperIgnoreTarget(nameof(GroupAssignment.Admin))]
    public partial GroupAssignment Map(Shared.Messages.Groups.GroupAssignmentDto groupAssignmentDto);

    public partial StudentDto Map(Student student);

    public partial Admin Map(Shared.Messages.Admins.AdminDto adminDto);
    public partial Student Map(Shared.Messages.Students.StudentDto adminDto);

    public partial ChatDto Map(Chatter.Models.Chat chat);
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