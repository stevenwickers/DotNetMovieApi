namespace DotNetMovieApi.Contracts.Requests;

public sealed class MovieRequest
{
    public required string MovieName { get; init; }
    public required DateOnly ReleaseDate { get; init; }
    public decimal? WorldwideGross { get; init; }
    public decimal? ProductionBudget { get; init; }
    public decimal? DomesticGross { get; init; }
    public string[] Genres { get; init; } = [];
}