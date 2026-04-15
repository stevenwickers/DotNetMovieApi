using System.Text.Json;
using Dapper;
using DotNetMovieApi.Contracts.Support;
using DotNetMovieApi.Contracts.Requests;
using DotNetMovieApi.Contracts.Responses;
using DotNetMovieApi.Data;
using DotNetMovieApi.Repositories.Interfaces;
using HotChocolate;

namespace DotNetMovieApi.Repositories;

public sealed class MovieRepository : IMovieRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public MovieRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<MovieResponse>> Select(
        MovieFilters? filters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        filters ??= new MovieFilters();

        var resolvedSort = MovieSortResolver.Resolve(filters.SortBy, filters.SortDirection);
        var isPaged = filters.Page.HasValue && filters.PageSize.HasValue;

        var parameters = BuildMovieParameters(filters, resolvedSort);

        var moviesCommand = new CommandDefinition(
            commandText: """
                SELECT *
                FROM wickers.get_movies(
                    p_search => @Search,
                    p_search_mode => @Mode,
                    p_page => @Page,
                    p_page_size => @PageSize,
                    p_sort_by => @SortBy,
                    p_sort_direction => @SortDirection,
                    p_release_date_from => @ReleaseDateFrom,
                    p_release_date_to => @ReleaseDateTo,
                    p_worldwide_gross_min => @WorldwideGrossMin,
                    p_worldwide_gross_max => @WorldwideGrossMax,
                    p_production_budget_min => @ProductionBudgetMin,
                    p_production_budget_max => @ProductionBudgetMax,
                    p_domestic_gross_min => @DomesticGrossMin,
                    p_domestic_gross_max => @DomesticGrossMax,
                    p_genres => @Genres
                );
                """,
            parameters: parameters,
            cancellationToken: cancellationToken
        );

        var totalCountCommand = new CommandDefinition(
            commandText: """
                SELECT wickers.get_movies_count(
                    p_search => @Search,
                    p_search_mode => @Mode,
                    p_release_date_from => @ReleaseDateFrom,
                    p_release_date_to => @ReleaseDateTo,
                    p_worldwide_gross_min => @WorldwideGrossMin,
                    p_worldwide_gross_max => @WorldwideGrossMax,
                    p_production_budget_min => @ProductionBudgetMin,
                    p_production_budget_max => @ProductionBudgetMax,
                    p_domestic_gross_min => @DomesticGrossMin,
                    p_domestic_gross_max => @DomesticGrossMax,
                    p_genres => @Genres
                );
                """,
            parameters: parameters,
            cancellationToken: cancellationToken
        );

        var movies = (await connection.QueryAsync<MovieResponse>(moviesCommand)).ToList();
        var totalCount = await connection.ExecuteScalarAsync<int>(totalCountCommand);

        var safePage = filters.Page ?? 1;
        var safePageSize = filters.PageSize ?? Math.Max(movies.Count, 1);
        var totalPages = isPaged
            ? (int)Math.Ceiling(totalCount / (double)safePageSize)
            : 1;

        return new PagedResponse<MovieResponse>
        {
            Items = movies,
            Page = safePage,
            PageSize = safePageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            SortBy = resolvedSort.SortBy,
            SortDirection = resolvedSort.SortDirection,
            UsedDefaultSort = resolvedSort.UsedDefaultSort
        };
    }

    public async Task<MovieResponse?> SelectById(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "SELECT * FROM wickers.get_movie_by_id(@Id);",
            parameters: new { Id = id },
            cancellationToken: cancellationToken
        );

        return await connection.QueryFirstOrDefaultAsync<MovieResponse>(command);
    }

    public async Task<Guid> Create(MovieRequest request, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: """
                SELECT * FROM wickers.create_movie(
                    @MovieName,
                    @ReleaseDate,
                    @WorldwideGross,
                    @ProductionBudget,
                    @DomesticGross,
                    @Genres
                );
                """,
            parameters: new
            {
                request.MovieName,
                request.ReleaseDate,
                request.WorldwideGross,
                request.ProductionBudget,
                request.DomesticGross,
                Genres = request.Genres
            },
            cancellationToken: cancellationToken
        );

        return await connection.ExecuteScalarAsync<Guid>(command);
    }

    public async Task<bool> Update(Guid id, MovieRequest request, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: """
                SELECT * FROM wickers.update_movie(
                    @Id,
                    @MovieName,
                    @ReleaseDate,
                    @WorldwideGross,
                    @ProductionBudget,
                    @DomesticGross,
                    @Genres
                );
                """,
            parameters: new
            {
                Id = id,
                request.MovieName,
                request.ReleaseDate,
                request.WorldwideGross,
                request.ProductionBudget,
                request.DomesticGross,
                Genres = request.Genres
            },
            cancellationToken: cancellationToken
        );

        var movie = await connection.QueryFirstOrDefaultAsync<MovieResponse>(command);

        return movie is not null;
    }

    public async Task<MovieResponse?> Patch(
        Guid id,
        MoviePatchRequest patch,
        CancellationToken cancellationToken = default)
    {
        var patchPayload = BuildPatchPayload(patch);

        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("MovieId", id);
        parameters.Add("Patch", JsonSerializer.Serialize(patchPayload));

        var command = new CommandDefinition(
            commandText: """
                SELECT *
                FROM wickers.update_graphql_movie(
                    @MovieId,
                    CAST(@Patch AS jsonb)
                );
                """,
            parameters: parameters,
            cancellationToken: cancellationToken
        );

        return await connection.QueryFirstOrDefaultAsync<MovieResponse>(command);
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "SELECT wickers.delete_movie(@Id);",
            parameters: new { Id = id },
            cancellationToken: cancellationToken
        );

        return await connection.ExecuteScalarAsync<bool>(command);
    }

    private static object BuildMovieParameters(MovieFilters filters, dynamic resolvedSort)
    {
        var normalizedGenres = filters.Genres is { Length: > 0 }
            ? filters.Genres
            : null;
        var normalizedSearchMode = string.IsNullOrWhiteSpace(filters.Search)
            ? null
            : filters.ParsedSearchMode?.ToString().ToLowerInvariant() ?? "general";

        return new
        {
            Search = filters.Search,
            Mode = normalizedSearchMode,
            Page = filters.Page,
            PageSize = filters.PageSize,
            SortBy = resolvedSort.SortBy,
            SortDirection = resolvedSort.SortDirection,
            ReleaseDateFrom = filters.ReleaseDateFrom,
            ReleaseDateTo = filters.ReleaseDateTo,
            WorldwideGrossMin = filters.WorldwideGrossMin,
            WorldwideGrossMax = filters.WorldwideGrossMax,
            ProductionBudgetMin = filters.ProductionBudgetMin,
            ProductionBudgetMax = filters.ProductionBudgetMax,
            DomesticGrossMin = filters.DomesticGrossMin,
            DomesticGrossMax = filters.DomesticGrossMax,
            Genres = normalizedGenres
        };
    }

    private static Dictionary<string, object?> BuildPatchPayload(MoviePatchRequest patch)
    {
        var payload = new Dictionary<string, object?>();

        AddIfSpecified(payload, "movie_name", patch.MovieName);
        AddIfSpecified(payload, "release_date", patch.ReleaseDate, value => value?.ToString("yyyy-MM-dd"));
        AddIfSpecified(payload, "worldwide_gross", patch.WorldwideGross);
        AddIfSpecified(payload, "production_budget", patch.ProductionBudget);
        AddIfSpecified(payload, "domestic_gross", patch.DomesticGross);
        AddIfSpecified(payload, "genre_names", patch.GenreNames, value => value?.ToArray());

        return payload;
    }

    private static void AddIfSpecified<T>(
        IDictionary<string, object?> payload,
        string key,
        Optional<T> optional,
        Func<T, object?>? transform = null)
    {
        if (!optional.HasValue)
        {
            return;
        }

        payload[key] = transform is null
            ? optional.Value
            : transform(optional.Value);
    }
}
