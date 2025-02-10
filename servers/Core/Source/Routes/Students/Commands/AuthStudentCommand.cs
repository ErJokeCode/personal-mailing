using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.External.Parser;
using Core.Models;
using Core.Routes.Students.Dtos;
using Core.Routes.Students.Errors;
using Core.Routes.Students.Maps;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Students.Commands;

public class AuthStudentCommand : IRequest<Result<StudentDto>>
{
    public required string Email { get; set; }
    public required string PersonalNumber { get; set; }
    public required string ChatId { get; set; }
}

public class AuthStudentCommandHandler : IRequestHandler<AuthStudentCommand, Result<StudentDto>>
{
    private readonly AppDbContext _db;
    private readonly IParser _parser;
    private readonly StudentMapper _mapper;

    public AuthStudentCommandHandler(AppDbContext db, IParser parser)
    {
        _db = db;
        _parser = parser;
        _mapper = new StudentMapper();
    }

    public async Task<Result<StudentDto>> Handle(AuthStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _db.Students.SingleOrDefaultAsync(s => s.Email == request.Email, cancellationToken);

        if (student is not null)
        {
            if (!await CanAuth(request, student))
            {
                return Result.Fail<StudentDto>(StudentErrors.AuthError());
            }

            return Result.Ok(_mapper.Map(student));
        }

        student = new Student()
        {
            Email = request.Email,
            ChatId = request.ChatId,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        if (!await CanAuth(request, student))
        {
            return Result.Fail<StudentDto>(StudentErrors.AuthError());
        }

        await _db.Students.AddAsync(student, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Ok(_mapper.Map(student));
    }

    private async Task<bool> CanAuth(AuthStudentCommand request, Student student)
    {
        var found = await _parser.IncludeInfoAsync(student);

        if (!found || student.Info!.PersonalNumber != request.PersonalNumber)
        {
            return false;
        }

        return true;
    }
}