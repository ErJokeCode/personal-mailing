using System.Threading.Tasks;
using MassTransit;
using Notify.Data;
using Notify.Routes.Notifications.Maps;
using Shared.Context.Students.Messages;

namespace Notify.Features.Students.Consumers;

class StudentsUpdatedBulkConsumer : IConsumer<StudentsUpdatedBulkMessage>
{
    private readonly AppDbContext _db;
    private readonly StudentMapper _studentMapper;

    public StudentsUpdatedBulkConsumer(AppDbContext db, StudentMapper studentMapper)
    {
        _db = db;
        _studentMapper = studentMapper;
    }

    public async Task Consume(ConsumeContext<StudentsUpdatedBulkMessage> context)
    {
        foreach (var studentUpdate in context.Message.Students)
        {
            var student = await _db.Students.FindAsync(studentUpdate.Id);

            if (student is not null)
            {
                student = _studentMapper.Map(studentUpdate);
            }
        }

        await _db.SaveChangesAsync();
    }
}
