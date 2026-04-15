namespace DotNetMovieApi.Contracts.Support;

public static class MovieSortResolver
{
    public static readonly string[] AllowedSortColumns =
    [
        "movie_name",
        "release_date",
        "worldwide_gross",
        "production_budget",
        "domestic_gross"
    ];

    private static readonly HashSet<string> AllowedSortColumnsLookup = new(AllowedSortColumns, StringComparer.OrdinalIgnoreCase);

    public static bool IsAllowedSortColumn(string? sortBy)
    {
        return string.IsNullOrWhiteSpace(sortBy) || AllowedSortColumnsLookup.Contains(sortBy);
    }

    public static ResolvedSort Resolve(string? sortBy, string? sortDirection)
    {
        var usedDefaultSort = !IsAllowedSortColumn(sortBy);

        var resolvedSortBy = usedDefaultSort
            ? "movie_name"
            : sortBy!;

        var resolvedSortDirection =
            string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase)
                ? "DESC"
                : "ASC";

        return new ResolvedSort(
            resolvedSortBy,
            resolvedSortDirection,
            usedDefaultSort
        );
    }
}
