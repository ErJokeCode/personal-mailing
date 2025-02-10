using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.External.Parser;
using Core.Models;
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
    private readonly IParser _parser;

    public GetAllStudentsQueryHandler(AppDbContext db, IParser parser)
    {
        _db = db;
        _studentMapper = new StudentMapper();
        _parser = parser;
    }

    public async Task<IEnumerable<StudentDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = await _db.Students.ToListAsync(cancellationToken);

        await _parser.IncludeInfoAsync(students);

        students = students.Where(s => s.Info is not null).ToList();

        return _studentMapper.Map(students);
    }
}