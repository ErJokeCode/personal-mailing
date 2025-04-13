using System.Threading.Tasks;
using Chatter.Data;
using Chatter.Features.Students;
using MassTransit;
using Shared.Context.Students.Messages;

namespace Chatter.Features.Students.Consumers;

class StudentAuthedConsumer : IConsumer<StudentAuthedMessage>
{
    private readonly AppDbContext _db;
    private readonly StudentMapper _studentMapper;

    public StudentAuthedConsumer(AppDbContext db, StudentMapper studentMapper)
    {
        _db = db;
        _studentMapper = studentMapper;
    }

    public async Task Consume(ConsumeContext<StudentAuthedMessage> context)
    {
        var exists = await _db.Students.FindAsync(context.Message.Id);

        if (exists != null)
        {
            return;
        }

        var student = _studentMapper.Map(context.Message);

        await _db.Students.AddAsync(student);
        await _db.SaveChangesAsync();
    }
}