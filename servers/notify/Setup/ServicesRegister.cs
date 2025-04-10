using System;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Notify.Services.MailService;
using Notify.Services.UserAccessor;
using Shared.Services.FileStorage;

namespace Notify.Setup;

public static class ServicesRegister
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IUserAccessor, UserAccessor>();

        services.Configure<MailServiceOptions>(o =>
        {
            o.MailServiceUrl = Environment.GetEnvironmentVariable("BOT_URL")!;
        });
        services.AddScoped<IMailService, TelegramBot>();

        services.AddMinio(
            configureClient => configureClient
                .WithEndpoint(Environment.GetEnvironmentVariable("MINIO_URL"))
                .WithCredentials(Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY"), Environment.GetEnvironmentVariable("MINIO_SECRET_KEY"))
                .WithSSL(false)
                .Build()
        );

        services.Configure<FileStorageOptions>(o =>
        {
            o.BucketName = Environment.GetEnvironmentVariable("MINIO_BUCKET")!;
        });
        services.AddSingleton<IFileStorage, MinioStorage>();
    }
}