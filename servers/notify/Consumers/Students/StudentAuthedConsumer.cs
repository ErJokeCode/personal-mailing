using System;
using System.Threading.Tasks;
using MassTransit;
using Notify.Abstractions.Parser;
using Notify.Data;
using Notify.Models;

namespace Notify.Consumers.Students;

public class StudentDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string ChatId { get; set; }
    public required DateOnly CreatedAt { get; set; }

    public required ParserStudent Info { get; set; }

    public bool Active { get; set; }
    public DateOnly? DeactivatedAt { get; set; }
}

public class StudentAuthedMessage
{
    public required StudentDto Student { get; set; }
}

class StudentAuthedConsumer : IConsumer<StudentAuthedMessage>
{
    private readonly AppDbContext _db;

    public StudentAuthedConsumer(AppDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<StudentAuthedMessage> context)
    {
        var exists = await _db.Students.FindAsync(context.Message.Student.Id);

        if (exists != null)
        {
            return;
        }

        var studentDto = context.Message.Student;

        var student = new Student()
        {
            CreatedAt = studentDto.CreatedAt,
            Email = studentDto.Email,
            Id = studentDto.Id,
            ChatId = studentDto.ChatId,
            Info = studentDto.Info,
            Active = studentDto.Active,
            DeactivatedAt = studentDto.DeactivatedAt,
        };

        await _db.Students.AddAsync(student);
        await _db.SaveChangesAsync();
    }
}