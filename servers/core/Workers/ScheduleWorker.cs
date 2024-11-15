
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Workers;

public class ScheduleWorker : BackgroundService, IDisposable
{
    private readonly IServiceScopeFactory _services;
    private Timer _timer;

    public ScheduleWorker(IServiceScopeFactory scopeFactory)
    {
        _services = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer((state) => SendScheduled(_services), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    private void SendScheduled(IServiceScopeFactory scopeFactory)
    {
        using var scope = _services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<CoreDb>();

        var count = context.NotificationSchedules.Count();
        var now = DateTime.Now;

        Console.WriteLine(count);
        Console.WriteLine(now.ToString());
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _timer.Dispose();
    }
}
