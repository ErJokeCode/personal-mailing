
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Routes.Students.Dtos;
using Core.Routes.Students.Maps;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Students.Queries;

public class GetStudentByIdQuery : IRequest<Result<StudentDto>>
{
    public Guid StudentId { get; init; }
}

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentDto>>
{
    private readonly AppDbContext _db;
    private readonly StudentMapper _studentMapper;

    public GetStudentByIdQueryHandler(AppDbContext db)
    {
        _db = db;
        _studentMapper = new StudentMapper();
    }

    public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await _db.Students.SingleOrDefaultAsync(s => s.Id == request.StudentId);

        if (student is null)
        {
            return Result.Fail<StudentDto>($"Студент с айди {request.StudentId} не был найден");
        }

        return Result.Ok(_studentMapper.Map(student));
    }
}