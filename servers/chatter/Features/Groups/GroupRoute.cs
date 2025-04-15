using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Shared.Infrastructure.Extensions;
using Chatter.Features.Groups.Queries;
using Chatter.Features.Groups.Commands;

namespace Chatter.Features.Groups;

public class GroupRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/chatter/groups")
            .WithTags("Группы");

        group.MapGet("/", GetAllGroupsQuery.Handle)
            .WithDescription("Получет все привязывания групп");

        group.MapPatch("/", AssignGroupsCommand.Handle)
            .WithDescription("Привязывает группы к админу");
    }
}