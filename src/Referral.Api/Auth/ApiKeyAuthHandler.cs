using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Referral.Api.Auth;

/// <summary>
/// This is a simplify api auth implementation for mocking purposes. For a service to service
/// the auth will most likely use jwt token from an idp. This is added for mocking
/// and testing secure endpoints.
/// </summary>
public class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthSchemeOptions>
{
    public ApiKeyAuthHandler(IOptionsMonitor<ApiKeyAuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    public ApiKeyAuthHandler(IOptionsMonitor<ApiKeyAuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));

        var header = Request.Headers[HeaderNames.Authorization].ToString();
        if (header != Constants.ApiKeySchemeName)
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));

        var claims = new[]
        {
            new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString("n")),
            new Claim(ClaimTypes.Email, "mockapiuser@test.com"),
            new Claim(ClaimTypes.Name, "Mock User"),
        };
        var claimsIdentity = new ClaimsIdentity(claims, "ApiKey");

        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(claimsIdentity), Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
