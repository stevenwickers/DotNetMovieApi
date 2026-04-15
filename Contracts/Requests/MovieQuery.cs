using DotNetMovieApi.Contracts.Enums;

namespace DotNetMovieApi.Contracts.Requests;

public sealed class MovieQuery
{
    public string? Search { get; init; }
    public SearchMode? SearchMode { get; init; }

    public int? Page { get; init; }
    public int? PageSize { get; init; }

    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }

    public DateOnly? ReleaseDateFrom { get; init; }
    public DateOnly? ReleaseDateTo { get; init; }

    public decimal? WorldwideGrossMin { get; init; }
    public decimal? WorldwideGrossMax { get; init; }

    public decimal? ProductionBudgetMin { get; init; }
    public decimal? ProductionBudgetMax { get; init; }

    public decimal? DomesticGrossMin { get; init; }
    public decimal? DomesticGrossMax { get; init; }

    public string[]? Genres { get; init; }
}
