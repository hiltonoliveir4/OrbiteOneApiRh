using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Api.Auth;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string HeaderName = "X-API-KEY";
    private const string AuthErrorKey = "ApiKeyAuthError";
    private readonly IConfiguration _configuration;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration) : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderName, out var apiKey))
        {
            Context.Items[AuthErrorKey] = "X-API-KEY não informado";
            return Task.FromResult(AuthenticateResult.Fail("Missing API key"));
        }

        var expected = _configuration["Security:StaticSecret"];
        if (string.IsNullOrWhiteSpace(expected) || apiKey != expected)
        {
            Context.Items[AuthErrorKey] = "X-API-KEY inválido";
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));
        }

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "api-key") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var message = Context.Items.TryGetValue(AuthErrorKey, out var value)
            ? value?.ToString() ?? "Não autorizado"
            : "Não autorizado";

        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(new { message });
        return Response.WriteAsync(payload);
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        Response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(new { message = "Sem permissão" });
        return Response.WriteAsync(payload);
    }
}
