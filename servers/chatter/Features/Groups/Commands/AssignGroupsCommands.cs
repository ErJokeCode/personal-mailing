using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatter.Routes.Chats.Maps;
using FluentResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Chatter.Data;
using Chatter.Signal;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Errors;
using Shared.Context.Admins;
using Microsoft.AspNetCore.Http;

namespace Chatter.Features.Groups.Commands;

public static class AssignGroupsCommand
{
    public class Request
    {
        public required Guid AdminId { get; set; }
        public IEnumerable<int> GroupIds { get; set; } = [];
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.GroupIds)
                .NotEmpty()
                .WithName("Группы");
        }
    }

    public static async Task<Results<NoContent, NotFound<ProblemDetails>, ValidationProblem>> Handle(
        Request request,
        IValidator<Request> validator,
        AppDbContext db,
        IHubContext<SignalHub> hub,
        ChatMapper chatMapper
    )
    {
        if (validator.TryValidate(request, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var admin = await db.Users.FindAsync(request.AdminId);

        if (admin is null)
        {
            return Result.Fail(AdminErrors.NotFound(request.AdminId)).ToNotFoundProblem();
        }

        var groups = await db.GroupAssignments
            .Where(g => request.GroupIds.Contains(g.Id))
            .ToListAsync();

        foreach (var group in groups)
        {
            var oldAdmin = await db.Users
                .Include(a => a.Chats)
                .ThenInclude(ch => ch.Student)
                .SingleAsync(a => a.Id == group.AdminId);

            var chats = oldAdmin.Chats.Where(ch => ch.Student!.Info.Group.Number == group.Name);

            foreach (var chat in chats)
            {
                chat.AdminId = request.AdminId;

                if (chat.UnreadCount > 0)
                {
                    await hub.NotifyOfChatRead(request.AdminId, chatMapper.Map(chat));
                }
            }

            group.AdminId = request.AdminId;
        }

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}