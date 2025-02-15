using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

// TODO add method to get an admins assigned groups

public class GetAllGroupsQuery : IRequest<IEnumerable<GroupAssignmentDto>>;

public class GetAllGroupsQueryHandler : IRequestHandler<GetAllGroupsQuery, IEnumerable<GroupAssignmentDto>>
{
    private readonly AppDbContext _db;
    private readonly AdminMapper _adminMapper;

    public GetAllGroupsQueryHandler(AppDbContext db)
    {
        _db = db;
        _adminMapper = new AdminMapper();
    }

    public async Task<IEnumerable<GroupAssignmentDto>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await _db.GroupAssignments
            .Include(g => g.Admin)
            .ToListAsync();

        return _adminMapper.Map(groups);
    }
}
