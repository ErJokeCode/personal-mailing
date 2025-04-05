using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Routes.Chats.Maps;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Notify.Abstractions.Parser;
using Notify.Data;
using Notify.Models;
using Shared.Messages.Students;

namespace Notify.Consumers.Students;

class StudentsUpdatedConsumer : IConsumer<StudentsUpdatedMessage>
{
    private readonly AppDbContext _db;
    private readonly ChatMapper _chatMapper;

    public StudentsUpdatedConsumer(AppDbContext db)
    {
        _db = db;
        _chatMapper = new ChatMapper();
    }

    public async Task Consume(ConsumeContext<StudentsUpdatedMessage> context)
    {
        foreach (var studentDto in context.Message.Students)
        {
            var student = await _db.Students.FindAsync(studentDto.Id);

            if (student is null)
            {
                var newStudent = _chatMapper.Map(studentDto);

                await _db.Students.AddAsync(newStudent);
            }
            else
            {
                student = _chatMapper.Map(studentDto);
            }
        }

        await _db.SaveChangesAsync();
    }
}
