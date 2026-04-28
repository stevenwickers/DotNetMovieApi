using Microsoft.AspNetCore.Diagnostics;
using DotNetMovieApi.Endpoints;
using DotNetMovieApi.Middleware;

namespace DotNetMovieApi.Configuration;

public static class EndpointRouteBuilderExtensions
{
    public static IApplicationBuilder UseAppMiddleware(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        
        app.UseExceptionHandler("/error");

        app.UseMiddleware<RequestLoggingMiddleware>();
        
        app.UseCors("FrontendCorsPolicy");
        
        app.UseSwagger(options => { });
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie API v1");
            options.RoutePrefix = "swagger";
        });
        
        return app;
    }

    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapMovieEndpoints();
        app.MapGenreEndpoints();
        app.MapGraphQL("/graphql");
        
        app.Map("/error", (HttpContext context, ILogger<Program> logger) =>
        {
            var exception = context.Features
                .Get<IExceptionHandlerFeature>()?
                .Error;

            var correlationId = context.Items[CorrelationIdMiddleware.ItemKey]?.ToString();
            
            if (exception is not null)
            {
                logger.LogError(
                    exception,
                    "Unhandled exception occurred. RequestId={CorrelationId}",
                    correlationId);
            }

            if (exception is BadHttpRequestException)
            {
                return Results.Problem(
                    title: "Invalid request body.",
                    detail: "Check that the JSON body is valid and uses supported field formats.",
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: new Dictionary<string, object?>
                    {
                        ["correlationId"] = correlationId
                    });
            }

            return Results.Problem(
                title: "An unexpected error occurred.",
                statusCode: StatusCodes.Status500InternalServerError,
                extensions: new Dictionary<string, object?>
                {
                    ["correlationId"] = correlationId
                });
        });

        app.MapGet("/", () => Results.Redirect("/swagger/index.html"))
            .ExcludeFromDescription();
        
        app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
            .WithName("HealthCheck")
            .WithSummary("Health check")
            .ExcludeFromDescription();
        
        return app;
    }
}
