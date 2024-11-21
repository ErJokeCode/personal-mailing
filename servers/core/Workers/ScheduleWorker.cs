
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Handlers;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.EntityFrameworkCore;
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
        _timer =
            new Timer(async (state) => await CheckSchedule(_services), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    private async Task CheckSchedule(IServiceScopeFactory scopeFactory)
    {
        using var scope = _services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<CoreDb>();

        var schedules = context.NotificationSchedules.ToList();
        var now = DateTime.UtcNow;

        foreach (var scheduled in schedules)
        {
            if (scheduled.Next <= now)
            {
                var diff = now - scheduled.Next;
                var intervals = (int)(diff / scheduled.Interval) + 1;

                scheduled.Next += intervals * scheduled.Interval;

                await NotificationHandler.SendFromTemplate(scheduled.TemplateId, context);
            }
        }

        context.SaveChanges();
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
