
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Infrastructure.Rest;
using Core.Models;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Admins.Queries;

public class GetAllAdminsQuery : IRequest<PagedList<AdminDto>>
{
    public string? Search { get; set; }

    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

public class GetAllAdminsQueryHandler : IRequestHandler<GetAllAdminsQuery, PagedList<AdminDto>>
{
    private readonly AppDbContext _db;
    private readonly AdminMapper _adminMapper;

    public GetAllAdminsQueryHandler(AppDbContext db)
    {
        _db = db;
        _adminMapper = new AdminMapper();
    }

    public async Task<PagedList<AdminDto>> Handle(GetAllAdminsQuery request, CancellationToken cancellationToken)
    {
        var admins = await _db.Users.ToListAsync(cancellationToken);

        admins = FilterAdmins(admins, request).ToList();

        return PagedList<AdminDto>.Create(_adminMapper.Map(admins), request.Page, request.PageSize);
    }

    public IEnumerable<Admin> FilterAdmins(IEnumerable<Admin> admins, GetAllAdminsQuery request)
    {
        if (!string.IsNullOrEmpty(request.Search))
        {
            admins = admins
                .Where(a => FuzzySearch.Contains(a.Email!, request.Search));
        }

        return admins;
    }
}