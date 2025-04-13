using System;
using System.Linq;
using System.Threading.Tasks;
using Chatter.Models;
using Chatter.Routes.Groups.DTOs;
using Microsoft.EntityFrameworkCore;
using Chatter.Data;
using Shared.Infrastructure.Rest;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;

namespace Chatter.Features.Groups.Queries;

public static class GetAllGroupsQuery
{
    public class Request
    {
        public string? Search { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        public Guid? AdminId { get; set; }
    }

    public static async Task<Ok<PagedList<GroupAssignmentDto>>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        GroupMapper groupMapper
    )
    {
        var groupsQuery = db.GroupAssignments
            .Include(g => g.Admin)
            .OrderBy(g => g.Name);

        var groups = await FilterGroups(groupsQuery, request).ToListAsync();

        var paged = PagedList<GroupAssignmentDto>.Create(groupMapper.Map(groups), request.Page, request.PageSize);

        return TypedResults.Ok(paged);
    }

    private static IQueryable<GroupAssignment> FilterGroups(IQueryable<GroupAssignment> groups, Request request)
    {
        if (!string.IsNullOrEmpty(request.Search))
        {
            groups = groups
                .Where(n => n.Name.ToLower().Contains(request.Search.ToLower()));
        }

        if (request.AdminId is not null)
        {
            groups = groups
                .Where(n => n.AdminId == request.AdminId);
        }

        return groups;
    }
}