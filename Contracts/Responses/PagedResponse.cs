namespace DotNetMovieApi.Contracts.Responses;

public sealed class PagedResponse<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }

    public string SortBy { get; init; } = string.Empty;
    public string SortDirection { get; init; } = string.Empty;
    public bool UsedDefaultSort { get; init; }
}