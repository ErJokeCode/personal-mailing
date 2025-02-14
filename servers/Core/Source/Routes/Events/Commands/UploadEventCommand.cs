using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions.Parser;
using Core.Data;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Routes.Events.Commands;

public class UploadEventCommand : IRequest<Unit>;

public class UploadEventCommandHandler : IRequestHandler<UploadEventCommand, Unit>
{
    private readonly AppDbContext _db;
    private readonly IParser _parser;
    private readonly IConfiguration _configuration;

    public UploadEventCommandHandler(AppDbContext db, IParser parser, IConfiguration configuration)
    {
        _db = db;
        _parser = parser;
        _configuration = configuration;
    }

    public async Task<Unit> Handle(UploadEventCommand request, CancellationToken cancellationToken)
    {
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

        // TODO HashSet for all groups and adminGroups
        // Use HashSet Contains to check for entry
        // Should be faster than rechecking every time, and they should not repeat, at all
        var groups = await _parser.GetAsync<IEnumerable<string>>("/student/number_groups", []);

        if (groups is not null)
        {
            var admins = await _db.Users.ToListAsync();
            var mainAdmin = admins.Single(a => a.Email == _configuration["MainAdmin:Name"]);

            foreach (var group in groups)
            {
                if (!admins.Any(a => a.Groups.Contains(group)))
                {
                    mainAdmin.Groups.Add(group);
                }
            }
        }

        await _db.SaveChangesAsync();

        return Unit.Value;
    }
}
