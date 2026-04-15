using DotNetMovieApi.Contracts;
using DotNetMovieApi.Contracts.Responses;

namespace DotNetMovieApi.Repositories.Interfaces;

public sealed class NoFilter;

public interface IGenreRepository :
    IFilterRepository<IEnumerable<GenreResponse>, NoFilter>,
    IReadByIdRepository<GenreResponse>
{
}