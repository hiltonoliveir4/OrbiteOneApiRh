using Application.Exceptions;
using System.Text.Json;

namespace Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.ContentType = "application/json";

            if (ex is OrbiteOneException appEx)
            {
                context.Response.StatusCode = appEx.StatusCode;
                await WriteMessage(context, appEx.Message);
                return;
            }

            _logger.LogError(ex, "Erro inesperado");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await WriteMessage(context, "Erro interno ao processar a requisição");
        }
    }

    private static Task WriteMessage(HttpContext context, string message)
    {
        var payload = JsonSerializer.Serialize(new { message });
        return context.Response.WriteAsync(payload);
    }
}
