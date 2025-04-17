using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shared.Context.Admins.Messages;

namespace Core.Setup;

public static class KafkaSetup
{
    private static string ServiceName => "core-";

    public static void SetupKafka(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddProducer<AdminCreatedMessage>(AdminCreatedMessage.TopicName);

                rider.UsingKafka((context, k) =>
                {
                    k.Host(Environment.GetEnvironmentVariable("KAFKA_URL"));
                });
            });
        });
    }
}