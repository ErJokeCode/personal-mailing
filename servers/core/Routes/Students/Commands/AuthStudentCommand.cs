using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions.Parser;
using Core.Data;
using Core.Models;
using Core.Routes.Students.Dtos;
using Core.Routes.Students.Maps;
using Core.Signal;
using FluentResults;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Errors.Students;
using Shared.Messages.Students;

namespace Core.Routes.Students.Commands;

public class AuthStudentCommand : IRequest<Result<Dtos.StudentDto>>
{
    public required string Email { get; set; }
    public required string PersonalNumber { get; set; }
    public required string ChatId { get; set; }
}

public class AuthStudentCommandHandler : IRequestHandler<AuthStudentCommand, Result<Dtos.StudentDto>>
{
    private readonly AppDbContext _db;
    private readonly IParser _parser;
    private readonly StudentMapper _mapper;
    private readonly IHubContext<SignalHub> _hub;
    private readonly ITopicProducer<StudentAuthedMessage> _topicProducer;

    public AuthStudentCommandHandler(AppDbContext db, IParser parser, IHubContext<SignalHub> hub, ITopicProducer<StudentAuthedMessage> topicProducer)
    {
        _db = db;
        _parser = parser;
        _mapper = new StudentMapper();
        _hub = hub;
        _topicProducer = topicProducer;
    }

    public async Task<Result<Dtos.StudentDto>> Handle(AuthStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _db.Students.SingleOrDefaultAsync(s => s.Email == request.Email, cancellationToken);

        if (student is not null)
        {
            if (!student.Active || !CanAuth(request, student))
            {
                return Result.Fail<Dtos.StudentDto>(StudentErrors.AuthError());
            }

            if (student.ChatId != request.ChatId)
            {
                student.ChatId = request.ChatId;
                await _db.SaveChangesAsync();
            }

            return Result.Ok(_mapper.Map(student));
        }

        var info = await _parser.GetInfoAsync(request.Email);

        if (info is null)
        {
            return Result.Fail<Dtos.StudentDto>(StudentErrors.AuthError());
        }

        student = new Student()
        {
            Email = request.Email,
            ChatId = request.ChatId,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            Info = info,
        };

        if (!CanAuth(request, student))
        {
            return Result.Fail<Dtos.StudentDto>(StudentErrors.AuthError());
        }

        await _db.Students.AddAsync(student, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map(student);
        await _hub.NotifyOfStudentAuth(dto);

        await _topicProducer.Produce(new StudentAuthedMessage()
        {
            Student = _mapper.MapToMessage(student)
        });

        return Result.Ok(dto);
    }

    private static bool CanAuth(AuthStudentCommand request, Student student)
    {
        return request.Email == student.Email && request.PersonalNumber == student.Info.PersonalNumber;
    }
}