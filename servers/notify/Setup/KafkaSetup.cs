using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Notify.Features.Admins.Consumers;
using Notify.Features.Students.Consumers;
using Shared.Context;
using Shared.Context.Admins.Messages;
using Shared.Context.Students.Messages;

namespace Notify.Setup;

public static class KafkaSetup
{
    private static string ServiceName => "notify-";

    public static void SetupKafka(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<AdminCreatedConsumer>();
                rider.AddConsumer<StudentAuthedConsumer>();
                rider.AddConsumer<StudentsUpdatedBulkConsumer>();

                rider.UsingKafka((context, k) =>
                {
                    k.Host(Environment.GetEnvironmentVariable("KAFKA_URL"));

                    k.ConfigureTopic<AdminCreatedMessage, AdminCreatedConsumer>(context);
                    k.ConfigureTopic<StudentAuthedMessage, StudentAuthedConsumer>(context);
                    k.ConfigureTopic<StudentUpdatedMessage, StudentUpdatedConsumer>(context);
                    k.ConfigureTopic<StudentsUpdatedBulkMessage, StudentsUpdatedBulkConsumer>(context);
                });
            });
        });
    }

    private static void ConfigureTopic<TMessage, TConsumer>(this IKafkaFactoryConfigurator kafkaConfigurator, IRiderRegistrationContext context)
        where TMessage : class, IMessage
        where TConsumer : class, IConsumer<TMessage>
    {
        kafkaConfigurator.TopicEndpoint<TMessage>(TMessage.TopicName, ServiceName + TMessage.TopicName, e =>
        {
            e.CreateIfMissing();
            e.ConfigureConsumer<TConsumer>(context);
        });
    }
}