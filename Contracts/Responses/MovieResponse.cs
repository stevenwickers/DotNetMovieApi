namespace DotNetMovieApi.Contracts.Responses;

public sealed class MovieResponse
{
    public required Guid Id { get; init; }
    public required string MovieName { get; init; }
    public required DateOnly ReleaseDate { get; init; }
    public decimal? WorldwideGross { get; init; }
    public decimal? ProductionBudget { get; init; }
    public decimal? DomesticGross { get; init; }
    public string[] Genres { get; init; } = [];
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}