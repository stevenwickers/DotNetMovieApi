using HotChocolate;

namespace DotNetMovieApi.Contracts.Requests;

public sealed class MoviePatchRequest
{
    public Optional<string?> MovieName { get; init; }
    public Optional<DateOnly?> ReleaseDate { get; init; }
    public Optional<decimal?> WorldwideGross { get; init; }
    public Optional<decimal?> ProductionBudget { get; init; }
    public Optional<decimal?> DomesticGross { get; init; }
    public Optional<IReadOnlyList<string?>?> GenreNames { get; init; }
}
