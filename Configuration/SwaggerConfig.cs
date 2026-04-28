using System.Text.Json.Serialization;
using DotNetMovieApi.Json;
using Microsoft.OpenApi;

namespace DotNetMovieApi.Configuration;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.AllowTrailingCommas = true;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "C# Minimal Movie API", 
                Description = "A C# movie API exposing both REST and GraphQL endpoints for working with movie data.",
                Version = "v1"
            });

            options.OperationFilter<SearchModeParameterFilter>();
            options.UseInlineDefinitionsForEnums();
            options.SupportNonNullableReferenceTypes();
        });

        return services;
    }
}
