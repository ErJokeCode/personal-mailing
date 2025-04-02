using System;
using System.Threading.Tasks;
using MassTransit;
using Notify.Data;
using Notify.Models;

namespace Notify.Consumers.Admins;

public class AdminDto
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required DateOnly CreatedAt { get; set; }
}

public class AdminCreatedMessage
{
    public required AdminDto Admin { get; set; }
}

class AdminCreatedConsumer : IConsumer<AdminCreatedMessage>
{
    private readonly AppDbContext _db;

    public AdminCreatedConsumer(AppDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<AdminCreatedMessage> context)
    {
        var adminDto = context.Message.Admin;

        var admin = new Admin()
        {
            CreatedAt = adminDto.CreatedAt,
            Email = adminDto.Email,
            UserName = adminDto.UserName,
            Id = adminDto.Id,
        };

        await _db.Users.AddAsync(admin);
        await _db.SaveChangesAsync();
    }
}
