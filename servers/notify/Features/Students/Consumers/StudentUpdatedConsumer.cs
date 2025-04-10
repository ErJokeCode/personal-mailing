using System.Threading.Tasks;
using MassTransit;
using Notify.Data;
using Shared.Context.Students.Messages;

namespace Notify.Features.Students.Consumers;

class StudentUpdatedConsumer : IConsumer<StudentUpdatedMessage>
{
    private readonly AppDbContext _db;
    private readonly StudentMapper _studentMapper;

    public StudentUpdatedConsumer(AppDbContext db, StudentMapper studentMapper)
    {
        _db = db;
        _studentMapper = studentMapper;
    }

    public async Task Consume(ConsumeContext<StudentUpdatedMessage> context)
    {
        var student = await _db.Students.FindAsync(context.Message.Id);

        if (student is not null)
        {
            student = _studentMapper.Map(context.Message);
        }

        await _db.SaveChangesAsync();
    }
}
