using DotNetMovieApi.Contracts.Requests;
using DotNetMovieApi.Contracts.Responses;
namespace DotNetMovieApi.Repositories.Interfaces;

public interface IMovieRepository :
    IReadByIdRepository<MovieResponse>,
    IFilterRepository<PagedResponse<MovieResponse>, MovieFilters>,
    ICreateRepository<MovieRequest>,
    IUpdateRepository<MovieRequest>,
    IDeleteRepository
{
    Task<MovieResponse?> Patch(Guid id, MoviePatchRequest patch, CancellationToken cancellationToken = default);
}
