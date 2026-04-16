using DotNetMovieApi.Contracts.Support;
using DotNetMovieApi.Contracts.Requests;

namespace DotNetMovieApi.Validation;

public static class MovieRequestValidator
{
    public static Dictionary<string, string[]> Validate(MovieFilters filters)
    {
        var errors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        if (filters.Page.HasValue ^ filters.PageSize.HasValue)
        {
            AddError(errors, nameof(filters.Page), "Page and PageSize must be supplied together.");
            AddError(errors, nameof(filters.PageSize), "Page and PageSize must be supplied together.");
        }

        if (filters.Page is <= 0)
        {
            AddError(errors, nameof(filters.Page), "Page must be greater than 0.");
        }

        if (filters.PageSize is <= 0)
        {
            AddError(errors, nameof(filters.PageSize), "PageSize must be greater than 0.");
        }

        if (!string.IsNullOrWhiteSpace(filters.InvalidSearchMode))
        {
            AddError(errors, nameof(filters.SearchMode), "SearchMode must be one of: general, starts, ends, contains.");
        }

        if (!MovieSortResolver.IsAllowedSortColumn(filters.SortBy))
        {
            AddError(errors, nameof(filters.SortBy), $"SortBy must be one of: {string.Join(", ", MovieSortResolver.AllowedSortColumns)}.");
        }

        if (!string.IsNullOrWhiteSpace(filters.SortDirection) &&
            !string.Equals(filters.SortDirection, "asc", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(filters.SortDirection, "desc", StringComparison.OrdinalIgnoreCase))
        {
            AddError(errors, nameof(filters.SortDirection), "SortDirection must be either 'asc' or 'desc'.");
        }

        ValidateRange(errors, nameof(filters.ReleaseDateFrom), nameof(filters.ReleaseDateTo), filters.ReleaseDateFrom, filters.ReleaseDateTo);
        ValidateRange(errors, nameof(filters.WorldwideGrossMin), nameof(filters.WorldwideGrossMax), filters.WorldwideGrossMin, filters.WorldwideGrossMax);
        ValidateRange(errors, nameof(filters.ProductionBudgetMin), nameof(filters.ProductionBudgetMax), filters.ProductionBudgetMin, filters.ProductionBudgetMax);
        ValidateRange(errors, nameof(filters.DomesticGrossMin), nameof(filters.DomesticGrossMax), filters.DomesticGrossMin, filters.DomesticGrossMax);

        return errors.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray(), StringComparer.OrdinalIgnoreCase);
    }

    public static Dictionary<string, string[]> Validate(MoviePatchRequest request)
    {
        var errors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        if (!HasAnyValue(request))
        {
            AddError(errors, "body", "At least one field must be provided.");
        }

        return errors.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray(), StringComparer.OrdinalIgnoreCase);
    }

    private static bool HasAnyValue(MoviePatchRequest request)
    {
        return request.MovieName.HasValue
               || request.ReleaseDate.HasValue
               || request.WorldwideGross.HasValue
               || request.ProductionBudget.HasValue
               || request.DomesticGross.HasValue
               || request.GenreNames.HasValue;
    }

    private static void ValidateRange<T>(
        IDictionary<string, List<string>> errors,
        string minimumName,
        string maximumName,
        T? minimum,
        T? maximum)
        where T : struct, IComparable<T>
    {
        if (minimum.HasValue && maximum.HasValue && minimum.Value.CompareTo(maximum.Value) > 0)
        {
            AddError(errors, minimumName, $"{minimumName} must be less than or equal to {maximumName}.");
        }
    }

    private static void AddError(
        IDictionary<string, List<string>> errors,
        string key,
        string message)
    {
        if (!errors.TryGetValue(key, out var messages))
        {
            messages = [];
            errors[key] = messages;
        }

        messages.Add(message);
    }
}
