using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions.Parser;
using Core.Data;
using Core.Routes.Students.Dtos;
using Core.Routes.Students.Maps;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Shared.Context.Students;

namespace Core.Routes.Students.Queries;

public class GetStudentByIdQuery : IRequest<Result<StudentDto>>
{
    public Guid StudentId { get; init; }
}

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentDto>>
{
    private readonly AppDbContext _db;
    private readonly IParser _parser;
    private readonly StudentMapper _studentMapper;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetStudentByIdQueryHandler(AppDbContext db, IParser parser, IHttpContextAccessor contextAccessor)
    {
        _db = db;
        _studentMapper = new StudentMapper();
        _parser = parser;
        _contextAccessor = contextAccessor;
    }

    public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await _db.Students.SingleOrDefaultAsync(s => s.Id == request.StudentId);

        if (student is null || !student.Active)
        {
            return Result.Fail<StudentDto>(StudentErrors.NotFound(request.StudentId));
        }

        return Result.Ok(_studentMapper.Map(student));
    }
}