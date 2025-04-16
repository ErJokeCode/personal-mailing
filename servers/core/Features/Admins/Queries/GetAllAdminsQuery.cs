using System.Linq;
using System.Threading.Tasks;
using Core.Data;
using Core.Features.Admins.DTOs;
using Core.Models;
using Core.Routes.Admins.Maps;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Rest;

namespace Core.Features.Admins.Queries;

public static class GetAllAdminsQuery
{
    public class Request
    {
        public string? Search { get; set; }

        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }


    public static async Task<Ok<PagedList<AdminDto>>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        AdminMapper adminMapper
    )
    {
        var adminsQuery = db.Users;

        var admins = await FilterAdmins(adminsQuery, request).ToListAsync();

        var paged = PagedList<AdminDto>.Create(adminMapper.Map(admins), request.Page, request.PageSize);

        return TypedResults.Ok(paged);
    }

    public static IQueryable<Admin> FilterAdmins(IQueryable<Admin> admins, Request request)
    {
        if (!string.IsNullOrEmpty(request.Search))
        {
            admins = admins
                .Where(a => a.Email!.ToLower().Contains(request.Search));
        }

        return admins;
    }

}