using System;
using Chatter.Features.Admins.Consumers;
using Chatter.Features.Groups.Consumers;
using Chatter.Features.Students.Consumers;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shared.Context;
using Shared.Context.Admins.Messages;
using Shared.Context.Groups.Messages;
using Shared.Context.Students.Messages;

namespace Chatter.Setup;

public static class KafkaSetup
{
    private static string ServiceName => "chatter-";

    public static void SetupKafka(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<AdminCreatedConsumer>();

                rider.AddConsumer<StudentAuthedConsumer>();
                rider.AddConsumer<StudentUpdatedConsumer>();
                rider.AddConsumer<StudentsUpdatedBulkConsumer>();

                rider.AddConsumer<GroupAddedConsumer>();
                rider.AddConsumer<GroupsAddedBulkConsumer>();

                rider.UsingKafka((context, k) =>
                {
                    k.Host(Environment.GetEnvironmentVariable("KAFKA_URL"));

                    k.ConfigureTopic<AdminCreatedMessage, AdminCreatedConsumer>(context);

                    k.ConfigureTopic<StudentAuthedMessage, StudentAuthedConsumer>(context);
                    k.ConfigureTopic<StudentUpdatedMessage, StudentUpdatedConsumer>(context);
                    k.ConfigureTopic<StudentsUpdatedBulkMessage, StudentsUpdatedBulkConsumer>(context);

                    k.ConfigureTopic<GroupAddedMessage, GroupAddedConsumer>(context);
                    k.ConfigureTopic<GroupsAddedBulkMessage, GroupsAddedBulkConsumer>(context);
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
            e.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5)));
        });
    }
}