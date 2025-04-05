using System.Collections.Generic;
using System.Text.Json;
using Core.Models;
using Core.Routes.Chats.DTOs;
using Notify.Models;
using Notify.Routes.Notifications.DTOs;
using Riok.Mapperly.Abstractions;

namespace Core.Routes.Chats.Maps;

[Mapper]
public partial class ChatMapper
{
    public partial GroupAssignment Map(Shared.Messages.Groups.GroupAssignmentDto groupAssignmentDto);

    public partial StudentDto Map(Student student);

    public partial Admin Map(Shared.Messages.Admins.AdminDto adminDto);
    public partial Student Map(Shared.Messages.Students.StudentDto adminDto);

    public partial ChatDto Map(Core.Models.Chat chat);
    public partial IEnumerable<ChatDto> Map(IEnumerable<Core.Models.Chat> chats);

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