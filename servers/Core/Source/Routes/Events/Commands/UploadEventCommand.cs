using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions.Parser;
using Core.Data;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Events.Commands;

public class UploadEventCommand : IRequest<Unit>;

public class UploadEventCommandHandler : IRequestHandler<UploadEventCommand, Unit>
{
    private readonly AppDbContext _db;
    private readonly IParser _parser;

    public UploadEventCommandHandler(AppDbContext db, IParser parser)
    {
        _db = db;
        _parser = parser;
    }

    public async Task<Unit> Handle(UploadEventCommand request, CancellationToken cancellationToken)
    {
        var students = await _db.Students.ToListAsync();

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

        return Unit.Value;
    }
}
