using Core.Services.UserAccessor;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Setup;

public static class ServicesRegister
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<AdminService>();

        services.AddScoped<IUserAccessor, UserAccessor>();
    }
}