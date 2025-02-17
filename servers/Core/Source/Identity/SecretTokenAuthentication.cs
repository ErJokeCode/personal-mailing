using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Core.Identity;

public class SecretTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public SecretTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out var header))
        {
            var token = header.Parameter;

            if (token == "secrettoken")
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "SecretTokenUser") };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
            }
        }
        else
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}