
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Admins.Queries;

public class GetAllAdminsQuery : IRequest<IEnumerable<AdminDto>>;

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
        return _adminMapper.Map(admins);
    }
}