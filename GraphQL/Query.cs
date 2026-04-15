using DotNetMovieApi.Contracts.Requests;
using DotNetMovieApi.Contracts.Responses;
using DotNetMovieApi.Repositories.Interfaces;

namespace DotNetMovieApi.GraphQL;

public sealed class Query
{
    public Task<PagedResponse<MovieResponse>> GetMovies(
        [Service] IMovieRepository movieRepository,
        MovieFilters? filters,
        CancellationToken cancellationToken)
    {
        return movieRepository.Select(filters, cancellationToken);
    }

    public Task<MovieResponse?> GetMovieById(
        Guid id,
        [Service] IMovieRepository movieRepository,
        CancellationToken cancellationToken)
    {
        return movieRepository.SelectById(id, cancellationToken);
    }
}
