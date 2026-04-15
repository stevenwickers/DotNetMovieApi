using DotNetMovieApi.Contracts.Requests;
using DotNetMovieApi.Contracts.Responses;
using DotNetMovieApi.Repositories.Interfaces;

namespace DotNetMovieApi.GraphQL;

public sealed class Mutation
{
    public async Task<MovieResponse> UpdateMovie(
        Guid id,
        MoviePatchRequest patch,
        [Service] IMovieRepository movieRepository,
        CancellationToken cancellationToken)
    {
        var movie = await movieRepository.Patch(id, patch, cancellationToken);

        return movie ?? throw new InvalidOperationException($"Movie with id {id} was not updated.");
    }
}
