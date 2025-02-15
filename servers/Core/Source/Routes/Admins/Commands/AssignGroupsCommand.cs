using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Maps;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Admins.Commands;

public class AssignGroupsCommand : IRequest<Result>
{
    public required Guid AdminId { get; set; }
    public IEnumerable<int> GroupIds { get; set; } = [];
}

public class AssignGroupsCommandHandler : IRequestHandler<AssignGroupsCommand, Result>
{
    private readonly AppDbContext _db;
    private readonly AdminMapper _adminMapper;

    public AssignGroupsCommandHandler(AppDbContext db)
    {
        _db = db;
        _adminMapper = new AdminMapper();
    }

    public async Task<Result> Handle(AssignGroupsCommand request, CancellationToken cancellationToken)
    {
        var groups = await _db.GroupAssignments
            .Where(g => request.GroupIds.Contains(g.Id))
            .ToListAsync();

        foreach (var group in groups)
        {
            group.AdminId = request.AdminId;
        }

        await _db.SaveChangesAsync();

        return Result.Ok();
    }
}