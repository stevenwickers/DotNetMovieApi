namespace DotNetMovieApi.Contracts.Support;

public sealed record ResolvedSort(
    string SortBy,
    string SortDirection,
    bool UsedDefaultSort
);