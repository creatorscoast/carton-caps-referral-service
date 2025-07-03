using Dapper;
using Microsoft.Data.Sqlite;
using Referral.Api.Core;
using System.Data;

namespace Referral.Api.Data.Impl;

public sealed class SqlLiteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlLiteConnectionFactory(string connectionString)
    {
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
        SqlMapper.AddTypeHandler(new DateTimeOffsetTypeHandler());

        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        return connection;
    }
}
