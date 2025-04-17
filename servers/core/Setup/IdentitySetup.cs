using System;
using System.Threading.Tasks;
using Core.Data;
using Core.Identity;
using Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Setup;

public static class IdentitySetup
{
    public static void SetupIdentity(this IServiceCollection services)
    {
        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, SecretTokenAuthenticationHandler>("SecretTokenScheme", null);

        services.AddAuthorization(o =>
        {
            o.AddPolicy(SecretTokenAuthentication.Policy, (p) =>
            {
                p.RequireClaim(SecretTokenAuthentication.Claim);
            });

            o.DefaultPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("SecretTokenScheme", IdentityConstants.ApplicationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        services.AddIdentity<Admin, IdentityRole<Guid>>((o) =>
        {
            o.Password.RequireDigit = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequiredUniqueChars = 0;
            o.Password.RequiredLength = 1;
            // FIXME doesnt allow "admin" as email so comment out for now
            // options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(o =>
        {
            o.SlidingExpiration = true;
            o.SessionStore = new AdminTicketStore(services);

            o.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                },

                OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }
            };
        });
    }
}