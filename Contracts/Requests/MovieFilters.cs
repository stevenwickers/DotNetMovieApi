using DotNetMovieApi.Contracts.Enums;
using HotChocolate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotNetMovieApi.Contracts.Requests;

public sealed class MovieFilters
{
    private SearchMode? _parsedSearchMode;
    private string? _searchModeText;

    public string? Search { get; set; }

    [FromQuery(Name = "searchMode")]
    [GraphQLIgnore]
    [BindNever]
    public string? SearchModeText
    {
        get => _searchModeText;
        set => SetSearchMode(value);
    }

    [GraphQLName("searchMode")]
    public string? SearchMode
    {
        get => _searchModeText;
        set => SetSearchMode(value);
    }

    public SearchMode? ParsedSearchMode => _parsedSearchMode;

    public string? InvalidSearchMode { get; private set; }

    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }

    public DateOnly? ReleaseDateFrom { get; set; }
    public DateOnly? ReleaseDateTo { get; set; }
    public decimal? WorldwideGrossMin { get; set; }
    public decimal? WorldwideGrossMax { get; set; }
    public decimal? ProductionBudgetMin { get; set; }
    public decimal? ProductionBudgetMax { get; set; }
    public decimal? DomesticGrossMin { get; set; }
    public decimal? DomesticGrossMax { get; set; }
    public string[]? Genres { get; set; }

    private void SetSearchMode(string? value)
    {
        _searchModeText = value;
        InvalidSearchMode = null;
        _parsedSearchMode = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (Enum.TryParse<SearchMode>(value, ignoreCase: true, out var parsedMode))
        {
            _parsedSearchMode = parsedMode;
            return;
        }

        InvalidSearchMode = value;
    }
}
