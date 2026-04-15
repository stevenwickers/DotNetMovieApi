using Dapper;
using DotNetMovieApi.Repositories.Interfaces;
using DotNetMovieApi.Contracts;
using DotNetMovieApi.Contracts.Responses;
using DotNetMovieApi.Data;

namespace DotNetMovieApi.Repositories;

public sealed class GenreRepository : IGenreRepository
{
    private readonly DbConnectionFactory _connectionFactory;
    
    public GenreRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task<IEnumerable<GenreResponse>> Select(
        NoFilter? filters = null, 
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            "select * from wickers.get_genres();",
            cancellationToken: cancellationToken
        );

        return await connection.QueryAsync<GenreResponse>(command);
    }
    
    public async Task<GenreResponse?> SelectById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            "SELECT * FROM wickers.get_genre_by_id(@Id);",
            parameters: new { Id = id },
            cancellationToken: cancellationToken
        );

        return await connection.QueryFirstOrDefaultAsync<GenreResponse>(command);
    }
    
}