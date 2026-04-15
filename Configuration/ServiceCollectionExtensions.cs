using System.Text.Json.Serialization;
using DotNetMovieApi.Data;
using DotNetMovieApi.Json;
using DotNetMovieApi.Services;
using DotNetMovieApi.Repositories;
using DotNetMovieApi.Repositories.Interfaces;
using DotNetMovieApi.GraphQL;

namespace DotNetMovieApi.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        
        // Keep this for Minimal API JSON behavior
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.Converters.Add(new OptionalJsonConverterFactory());
        });
        
        var allowedOrigins =  configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy("FrontendCorsPolicy", policy =>
            {
                policy
                    .SetIsOriginAllowed(origin =>
                    {
                        if (allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
                        {
                            return true;
                        }

                        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                        {
                            return false;
                        }

                        return string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase)
                               || string.Equals(uri.Host, "127.0.0.1", StringComparison.OrdinalIgnoreCase);
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        
        services.AddHttpContextAccessor();
        services.AddScoped<RequestContextAccessor>();
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddSingleton<DbConnectionFactory>();
        services
            .AddGraphQLServer()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddType<DateType>();
        
        return services;
    }
}
