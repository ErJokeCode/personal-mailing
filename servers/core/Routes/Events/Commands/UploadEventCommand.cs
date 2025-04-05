using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions.Parser;
using Core.Data;
using Core.Models;
using Core.Routes.Students.Maps;
using Core.Signal;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Messages.Groups;
using Shared.Messages.Students;

namespace Core.Routes.Events.Commands;

public class UploadEventCommand : IRequest<Unit>;

public class UploadEventCommandHandler : IRequestHandler<UploadEventCommand, Unit>
{
    private readonly AppDbContext _db;
    private readonly IParser _parser;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<SignalHub> _hub;
    private readonly ITopicProducer<StudentsUpdatedMessage> _topicProducer;
    private readonly ITopicProducer<GroupsAddedMessage> _groupTopicProducer;
    private readonly StudentMapper _studentMapper;

    public UploadEventCommandHandler(AppDbContext db, IParser parser, IConfiguration configuration, IHubContext<SignalHub> hub, ITopicProducer<StudentsUpdatedMessage> topicProducer, ITopicProducer<GroupsAddedMessage> groupTopicProducer)
    {
        _studentMapper = new StudentMapper();
        _db = db;
        _parser = parser;
        _configuration = configuration;
        _hub = hub;
        _topicProducer = topicProducer;
        _groupTopicProducer = groupTopicProducer;
    }

    public async Task<Unit> Handle(UploadEventCommand request, CancellationToken cancellationToken)
    {
        var groups = await _parser.GetAsync<IEnumerable<string>>("/student/number_groups", []);
        var groupAssignments = await _db.GroupAssignments.Select(g => g.Name).ToHashSetAsync();

        List<GroupAssignment> newGroups = new();
        if (groups is not null)
        {
            var mainAdmin = await _db.Users.SingleAsync(a => a.Email == Environment.GetEnvironmentVariable("MAIN_ADMIN_EMAIL"));

            foreach (var group in groups)
            {
                if (!groupAssignments.Contains(group))
                {
                    var newGroup = new Models.GroupAssignment()
                    {
                        Name = group,
                        AdminId = mainAdmin.Id,
                    };

                    newGroups.Add(newGroup);

                    await _db.GroupAssignments.AddAsync(newGroup);
                }
            }
        }

        await _db.SaveChangesAsync();

        await _groupTopicProducer.Produce(new GroupsAddedMessage()
        {
            GroupAssignments = _studentMapper.Map(newGroups).ToList()
        });

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
            Students = _studentMapper.MapToMessage(students).ToList()
        });

        return Unit.Value;
    }
}
