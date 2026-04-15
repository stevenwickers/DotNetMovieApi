using Microsoft.AspNetCore.Mvc;
using DotNetMovieApi.Contracts.Enums;

namespace DotNetMovieApi.Contracts.Requests;

public sealed class MovieFilters
{
    private SearchMode? _parsedSearchMode;
    private string? _searchModeText;
    
    public string? Search { get; set; }

    [FromQuery(Name = "searchMode")]
    public string? SearchModeText
    {
        get => _searchModeText;
        set
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
}
