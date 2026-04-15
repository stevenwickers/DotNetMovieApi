using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DotNetMovieApi.Data;

public sealed class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres")
                            ?? throw new InvalidOperationException("Connection string 'Postgres' is missing.");
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}