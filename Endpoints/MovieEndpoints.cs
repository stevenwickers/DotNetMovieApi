using DotNetMovieApi.Repositories.Interfaces;
using DotNetMovieApi.Contracts.Requests;
using DotNetMovieApi.Contracts.Responses;
using DotNetMovieApi.Validation;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace DotNetMovieApi.Endpoints;

public static class MovieEndpoints
{
    public static IEndpointRouteBuilder MapMovieEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/movies").WithTags("Movies");

        group.MapGet("/", async (
            [AsParameters] MovieFilters query,
            [FromServices] IMovieRepository repo,
            [FromServices] ILoggerFactory loggerFactory,
            CancellationToken cancellationToken) =>
        {
            var validationErrors = MovieRequestValidator.Validate(query);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var logger = loggerFactory.CreateLogger("Movies");

            logger.LogInformation(
                "Fetching movies | Search: {Search} SearchMode: {SearchMode} Page: {Page} PageSize: {PageSize}",
                query.Search,
                query.SearchModeText,
                query.Page,
                query.PageSize);
            
            var movies = await repo.Select(
                query,
                cancellationToken);

            return Results.Ok(movies);
        }).WithDescription(
        """
         Examples:

         Basic:
         /movies

         Pagination:
         /movies?page=1&pageSize=10

         Search:
         /movies?search=avatar
         
         SearchMode is optional and defaults to general.
         Options: general | starts | ends | contains

         Filter:
         /movies?genres=Sci-Fi&releaseDateFrom=2000-01-01

         Combined:
         /movies?search=star&searchMode=starts&page=1&pageSize=10&genres=Sci-Fi

         SearchMode is optional and defaults to general.
         """
        );
        
        group.MapGet("/{id:guid}", async (
            Guid id, 
            IMovieRepository repo,
            CancellationToken cancellationToken) =>
        {
            var movie = await repo.SelectById(id, cancellationToken);
            return movie is null
                ? Results.NotFound(new ErrorResponse { Message = "Movie not found." })
                : Results.Ok(movie);
        });

        group.MapPost("/", async (
            MovieRequest request, 
            IMovieRepository repo, 
            CancellationToken cancellationToken) =>
        {
            try
            {
                var id = await repo.Create(request, cancellationToken);
                return Results.Created($"/movies/{id}", new IdResponse { Id = id });
            }
            catch (PostgresException exception)
                when (exception.SqlState == PostgresErrorCodes.UniqueViolation &&
                      string.Equals(exception.ConstraintName, "movies_movie_name_key", StringComparison.Ordinal))
            {
                return Results.Conflict(new ErrorResponse
                {
                    Message = $"A movie named '{request.MovieName}' already exists."
                });
            }
        });

        group.MapPut("/{id:guid}", async (
            Guid id, 
            MovieRequest request, 
            IMovieRepository repo,
            CancellationToken cancellationToken) =>
        {
            var updated = await repo.Update(id, request,  cancellationToken);
            if (!updated)
            {
                return Results.NotFound(new ErrorResponse { Message = "Movie not found." });
            }

            var movie = await repo.SelectById(id, cancellationToken);
            return Results.Ok(movie);
        });

        group.MapPatch("/{id:guid}", async (
                Guid id, 
                MoviePatchRequest request, 
                IMovieRepository repo,
                CancellationToken cancellationToken) =>
        {
            var validationErrors = MovieRequestValidator.Validate(request);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var patched = await repo.Patch(id, request, cancellationToken);
            if (patched == null)
            {
                return Results.NotFound(new ErrorResponse { Message = "Movie not found." });
            }

            return Results.Ok(patched);
        }).WithDescription(
            """
            Examples: 
            GraphQL Updates - Use this endpoint with GraphQL to update part of the record data

            Basic:
            {
                "worldwideGross": 1111,
                "productionBudget": 1
            }
            """
        )
        .Accepts<MoviePatchRequestBody>("application/json");

        group.MapDelete("/{id:guid}", async (
            Guid id, 
            IMovieRepository repo,
            CancellationToken cancellationToken) =>
        {
            var deleted = await repo.Delete(id, cancellationToken);
            return Results.Ok(new { deleted });
        });

        return app;
    }
}
