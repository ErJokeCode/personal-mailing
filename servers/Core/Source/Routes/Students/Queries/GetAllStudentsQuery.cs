using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Routes.Students.Dtos;
using Core.Routes.Students.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Students.Queries;

public class GetAllStudentsQuery : IRequest<IEnumerable<StudentDto>>;

public class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, IEnumerable<StudentDto>>
{
    private readonly AppDbContext _db;
    private readonly StudentMapper _studentMapper;

    public GetAllStudentsQueryHandler(AppDbContext db)
    {
        _db = db;
        _studentMapper = new StudentMapper();
    }

    public async Task<IEnumerable<StudentDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = await _db.Students
            .Where(s => s.Active)
            .ToListAsync(cancellationToken);

        return _studentMapper.Map(students);
    }
}