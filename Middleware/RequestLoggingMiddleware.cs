using System.Diagnostics;

namespace DotNetMovieApi.Middleware;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        await _next(context);
        
        stopwatch.Stop();
        
        var correlationId = context.Items[CorrelationIdMiddleware.ItemKey]?.ToString();
        
        _logger.LogInformation(
            "CorrelationId={CorrelationId} {Method} {Path} -> {StatusCode} ({ElapsedMilliseconds} ms)",
            correlationId,
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}