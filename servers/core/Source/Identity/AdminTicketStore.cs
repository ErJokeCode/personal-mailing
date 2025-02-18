using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Identity;

public class AdminTicketStore : ITicketStore
{
    private readonly IServiceCollection _services;

    public AdminTicketStore(IServiceCollection services)
    {
        _services = services;
    }

    public async Task RemoveAsync(string key)
    {
        using (var scope = _services.BuildServiceProvider().CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (Guid.TryParse(key, out var id))
            {
                var ticket = await context.AdminAuthTickets.SingleOrDefaultAsync(x => x.Id == id);

                if (ticket != null)
                {
                    context.AdminAuthTickets.Remove(ticket);
                    await context.SaveChangesAsync();
                }
            }
        }
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        using (var scope = _services.BuildServiceProvider().CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (Guid.TryParse(key, out var id))
            {
                var authenticationTicket = await context.AdminAuthTickets.FindAsync(id);

                if (authenticationTicket != null)
                {
                    authenticationTicket.Value = SerializeToBytes(ticket);
                    authenticationTicket.Expires = ticket.Properties.ExpiresUtc;
                    await context.SaveChangesAsync();
                }
            }
        }
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        using (var scope = _services.BuildServiceProvider().CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (Guid.TryParse(key, out var id))
            {
                var authenticationTicket = await context.AdminAuthTickets.FindAsync(id);

                if (authenticationTicket != null)
                {
                    return DeserializeFromBytes(authenticationTicket.Value);
                }
            }
        }

        return null;
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var userId = Guid.NewGuid();
        var nameIdentifier = Guid.Parse(ticket.Principal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value);

        using (var scope = _services.BuildServiceProvider().CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (ticket.AuthenticationScheme == "Identity.Application")
            {
                userId = nameIdentifier;
            }

            var authenticationTicket = new AdminAuthTicket()
            {
                AdminId = userId,
                Value = SerializeToBytes(ticket),
            };

            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc.HasValue)
            {
                authenticationTicket.Expires = expiresUtc.Value;
            }

            await context.AdminAuthTickets.AddAsync(authenticationTicket);
            await context.SaveChangesAsync();

            return authenticationTicket.Id.ToString();
        }
    }

    private byte[] SerializeToBytes(AuthenticationTicket source)
        => TicketSerializer.Default.Serialize(source);

    private AuthenticationTicket? DeserializeFromBytes(byte[] source)
        => source == null ? null : TicketSerializer.Default.Deserialize(source);
}