using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions.Parser;
using Core.Data;
using Core.Messages.Students;
using Core.Models;
using Core.Routes.Students.Maps;
using Core.Signal;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Routes.Events.Commands;

public class UploadEventCommand : IRequest<Unit>;

public class UploadEventCommandHandler : IRequestHandler<UploadEventCommand, Unit>
{
    private readonly AppDbContext _db;
    private readonly IParser _parser;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<SignalHub> _hub;
    private readonly ITopicProducer<StudentsUpdatedMessage> _topicProducer;
    private readonly StudentMapper _studentMapper;

    public UploadEventCommandHandler(AppDbContext db, IParser parser, IConfiguration configuration, IHubContext<SignalHub> hub, ITopicProducer<StudentsUpdatedMessage> topicProducer)
    {
        _studentMapper = new StudentMapper();
        _db = db;
        _parser = parser;
        _configuration = configuration;
        _hub = hub;
        _topicProducer = topicProducer;
    }

    public async Task<Unit> Handle(UploadEventCommand request, CancellationToken cancellationToken)
    {
        var groups = await _parser.GetAsync<IEnumerable<string>>("/student/number_groups", []);
        var groupAssignments = await _db.GroupAssignments.Select(g => g.Name).ToHashSetAsync();

        if (groups is not null)
        {
            var mainAdmin = await _db.Users.SingleAsync(a => a.Email == Environment.GetEnvironmentVariable("MAIN_ADMIN_EMAIL"));

            foreach (var group in groups)
            {
                if (!groupAssignments.Contains(group))
                {
                    await _db.GroupAssignments.AddAsync(new Models.GroupAssignment()
                    {
                        Name = group,
                        AdminId = mainAdmin.Id,
                    });
                }
            }
        }

        await _db.SaveChangesAsync();

        var students = await _db.Students.ToListAsync();

        // TODO implement a timestamp check if a student's data needs update
        // If timestamp is same, dont change anything
        // if it's newer, means it changed, recache data
        foreach (var student in students)
        {
            var info = await _parser.GetInfoAsync(student.Email);

            if (info is null)
            {
                student.Active = false;
                student.DeactivatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            }
            else
            {
                student.Active = true;
                student.DeactivatedAt = null;
                student.Info = info;
            }
        }

        await _db.SaveChangesAsync();

        await _hub.NotifyOfFileUpload();

        await _topicProducer.Produce(new StudentsUpdatedMessage()
        {
            Students = _studentMapper.Map(students).ToList()
        });

        return Unit.Value;
    }
}
