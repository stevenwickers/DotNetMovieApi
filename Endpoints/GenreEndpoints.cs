using DotNetMovieApi.Contracts.Responses;
using DotNetMovieApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DotNetMovieApi.Endpoints;

public static class GenreEndpoints
{
    public static IEndpointRouteBuilder MapGenreEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/genres").WithTags("Genre");

        group.MapGet("/", async (
            [FromServices] IGenreRepository repo,
            CancellationToken cancellationToken) =>
        {
            var genres = await repo.Select(null, cancellationToken);
            
            return Results.Ok(genres);
        });
       
        
        group.MapGet("/{id:guid}", async (
            Guid id, 
            [FromServices] IGenreRepository repo,
            CancellationToken cancellationToken) =>
        {
            var genre = await repo.SelectById(id, cancellationToken);
            return genre is null
                ? Results.NotFound(new ErrorResponse { Message = "Genre not found." })
                : Results.Ok(genre);
        });
        
        return app;
    }
} 
