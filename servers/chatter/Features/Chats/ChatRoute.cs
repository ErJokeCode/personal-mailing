using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Shared.Infrastructure.Extensions;
using Chatter.Features.Chats.Queries;
using Chatter.Features.Chats.Commands;

namespace Chatter.Features.Chats;

public class ChatRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/chatter/chats");

        group.MapGet("/", GetChatsQuery.Handle)
            .WithDescription("Получает чаты админа");

        group.MapPost("/", SendMessageCommand.Handle)
            .WithDescription("Отправляет сообщение студенту")
            .DisableAntiforgery();

        group.MapPost("/from-student", SendMessageFromStudentCommand.Handle)
            .WithDescription("Отправляет сообщение админу")
            .DisableAntiforgery();

        group.MapGet("/{studentId}", GetChatByIdQuery.Handle)
            .WithDescription("Получает чат со студентом по айди");

        group.MapPatch("/{studentId}/read", ReadChatCommand.Handle)
            .WithDescription("Делает чат прочитанным");

        group.MapGet("/{studentId}/messages", GetMessagesQuery.Handle)
            .WithDescription("Получает сообщения в чате");
    }
}