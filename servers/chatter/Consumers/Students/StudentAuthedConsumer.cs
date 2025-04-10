using System;
using System.Threading.Tasks;
using Chatter.Routes.Chats.Maps;
using MassTransit;
using Chatter.Data;
using Chatter.Models;
using Shared.Context.Students.Messages;

namespace Chatter.Consumers.Students;


class StudentAuthedConsumer : IConsumer<StudentAuthedMessage>
{
    private readonly AppDbContext _db;
    private readonly ChatMapper _chatMapper;

    public StudentAuthedConsumer(AppDbContext db)
    {
        _db = db;
        _chatMapper = new ChatMapper();
    }

    public async Task Consume(ConsumeContext<StudentAuthedMessage> context)
    {
        var exists = await _db.Students.FindAsync(context.Message.Student.Id);

        if (exists != null)
        {
            return;
        }

        var studentDto = context.Message.Student;
        var student = _chatMapper.Map(studentDto);

        await _db.Students.AddAsync(student);
        await _db.SaveChangesAsync();
    }
}