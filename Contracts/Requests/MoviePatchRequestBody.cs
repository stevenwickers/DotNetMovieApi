namespace DotNetMovieApi.Contracts.Requests;

public sealed class MoviePatchRequestBody
{
    public string? MovieName { get; init; }
    public DateOnly? ReleaseDate { get; init; }
    public decimal? WorldwideGross { get; init; }
    public decimal? ProductionBudget { get; init; }
    public decimal? DomesticGross { get; init; }
    public IReadOnlyList<string?>? GenreNames { get; init; }
}
