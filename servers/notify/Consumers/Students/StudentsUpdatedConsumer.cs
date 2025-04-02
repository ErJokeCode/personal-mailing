using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Notify.Abstractions.Parser;
using Notify.Data;
using Notify.Models;

namespace Notify.Consumers.Students;

public class StudentsUpdatedMessage
{
    public required List<StudentDto> Students { get; set; }
}

class StudentsUpdatedConsumer : IConsumer<StudentsUpdatedMessage>
{
    private readonly AppDbContext _db;

    public StudentsUpdatedConsumer(AppDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<StudentsUpdatedMessage> context)
    {
        foreach (var studentDto in context.Message.Students)
        {
            var student = await _db.Students.FindAsync(studentDto.Id);

            if (student is null)
            {
                var newStudent = new Student()
                {
                    CreatedAt = studentDto.CreatedAt,
                    Email = studentDto.Email,
                    Id = studentDto.Id,
                    ChatId = studentDto.ChatId,
                    Info = studentDto.Info,
                    Active = studentDto.Active,
                    DeactivatedAt = studentDto.DeactivatedAt,
                };

                await _db.Students.AddAsync(newStudent);
            }
            else
            {
                student.CreatedAt = studentDto.CreatedAt;
                student.Email = studentDto.Email;
                student.Id = studentDto.Id;
                student.ChatId = studentDto.ChatId;
                student.Info = studentDto.Info;
                student.Active = studentDto.Active;
                student.DeactivatedAt = studentDto.DeactivatedAt;
            }
        }

        await _db.SaveChangesAsync();
    }
}