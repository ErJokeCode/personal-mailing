using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Infrastructure.Search;
using Core.Models;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Maps;
using Core.Routes.Groups.DTOs;
using Core.Routes.Groups.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Groups.Queries;

public class GetAllGroupsQuery : IRequest<IEnumerable<GroupAssignmentDto>>
{
    public string? Name { get; set; }
    public Guid? AdminId { get; set; }
}

public class GetAllGroupsQueryHandler : IRequestHandler<GetAllGroupsQuery, IEnumerable<GroupAssignmentDto>>
{
    private readonly AppDbContext _db;
    private readonly GroupMapper _adminMapper;

    public GetAllGroupsQueryHandler(AppDbContext db)
    {
        _db = db;
        _adminMapper = new GroupMapper();
    }

    public async Task<IEnumerable<GroupAssignmentDto>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await _db.GroupAssignments
            .Include(g => g.Admin)
            .ToListAsync();

        groups = FilterGroups(groups, request).ToList();

        return _adminMapper.Map(groups);
    }

    private IEnumerable<GroupAssignment> FilterGroups(IEnumerable<GroupAssignment> groups, GetAllGroupsQuery request)
    {
        if (!string.IsNullOrEmpty(request.Name))
        {
            groups = groups
                .Where(n => FuzzySearch.Contains(n.Name, request.Name));
        }

        if (request.AdminId is not null)
        {
            groups = groups
                .Where(n => n.AdminId == request.AdminId);
        }

        return groups;
    }
}
