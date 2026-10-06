using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Infrastructure.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace AcmeIntegreatedAssessement.Infrastructure.Data
{
    public class SqlStoredProcedureInvoker : ISqlStoredProcedureInvoker
    {
        private readonly string _connectionString;

        public SqlStoredProcedureInvoker(IConnectionStringResolver resolver, IOptions<PaymentDatabaseSettings> options)
        {
            var settings = options.Value ?? throw new InvalidOperationException("Payment database settings are required.");
            var connectionStringName = string.IsNullOrWhiteSpace(settings.ConnectionStringName)
                ? PaymentDatabaseSettings.DefaultConnectionStringName
                : settings.ConnectionStringName;

            _connectionString = resolver.GetConnectionString(connectionStringName)
                ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' is required.");
        }

        private SqlCommand CreateCommand(SqlConnection connection, string procedureName)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = procedureName;
            command.CommandTimeout = 60;
            return command;
        }

        public async Task<int> ExecuteAsync(string procedureName, Action<SqlParameterCollection>? configureParameters = null, CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = CreateCommand(connection, procedureName);
            configureParameters?.Invoke(command.Parameters);
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task<T?> QuerySingleAsync<T>(string procedureName, Func<SqlDataReader, T> map, Action<SqlParameterCollection>? configureParameters = null, CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = CreateCommand(connection, procedureName);
            configureParameters?.Invoke(command.Parameters);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return map(reader);
            }
            return default;
        }

        public async Task<List<T>> QueryAsync<T>(string procedureName, Func<SqlDataReader, T> map, Action<SqlParameterCollection>? configureParameters = null, CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = CreateCommand(connection, procedureName);
            configureParameters?.Invoke(command.Parameters);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var results = new List<T>();
            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(map(reader));
            }
            return results;
        }
    }
}
