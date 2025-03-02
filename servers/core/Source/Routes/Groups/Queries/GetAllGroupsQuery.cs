using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Infrastructure.Rest;
using Core.Models;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Maps;
using Core.Routes.Groups.DTOs;
using Core.Routes.Groups.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Groups.Queries;

public class GetAllGroupsQuery : IRequest<PagedList<GroupAssignmentDto>>
{
    public string? Search { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public Guid? AdminId { get; set; }
}

public class GetAllGroupsQueryHandler : IRequestHandler<GetAllGroupsQuery, PagedList<GroupAssignmentDto>>
{
    private readonly AppDbContext _db;
    private readonly GroupMapper _adminMapper;

    public GetAllGroupsQueryHandler(AppDbContext db)
    {
        _db = db;
        _adminMapper = new GroupMapper();
    }

    public async Task<PagedList<GroupAssignmentDto>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await _db.GroupAssignments
            .Include(g => g.Admin)
            .OrderBy(g => g.Name)
            .ToListAsync();

        groups = FilterGroups(groups, request).ToList();

        return PagedList<GroupAssignmentDto>.Create(_adminMapper.Map(groups), request.Page, request.PageSize);
    }

    private IEnumerable<GroupAssignment> FilterGroups(IEnumerable<GroupAssignment> groups, GetAllGroupsQuery request)
    {
        if (!string.IsNullOrEmpty(request.Search))
        {
            groups = groups
                .Where(n => FuzzySearch.Contains(n.Name, request.Search));
        }

        if (request.AdminId is not null)
        {
            groups = groups
                .Where(n => n.AdminId == request.AdminId);
        }

        return groups;
    }
}
