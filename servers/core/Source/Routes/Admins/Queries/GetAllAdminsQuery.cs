
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Infrastructure.Search;
using Core.Models;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Admins.Queries;

public class GetAllAdminsQuery : IRequest<IEnumerable<AdminDto>>
{
    public string? Email { get; set; }
}

public class GetAllAdminsQueryHandler : IRequestHandler<GetAllAdminsQuery, IEnumerable<AdminDto>>
{
    private readonly AppDbContext _db;
    private readonly AdminMapper _adminMapper;

    public GetAllAdminsQueryHandler(AppDbContext db)
    {
        _db = db;
        _adminMapper = new AdminMapper();
    }

    public async Task<IEnumerable<AdminDto>> Handle(GetAllAdminsQuery request, CancellationToken cancellationToken)
    {
        var admins = await _db.Users.ToListAsync(cancellationToken);

        admins = FilterAdmins(admins, request).ToList();

        return _adminMapper.Map(admins);
    }

    public IEnumerable<Admin> FilterAdmins(IEnumerable<Admin> admins, GetAllAdminsQuery request)
    {
        if (!string.IsNullOrEmpty(request.Email))
        {
            admins = admins
                .Where(a => FuzzySearch.Contains(a.Email!, request.Email));
        }

        return admins;
    }
}