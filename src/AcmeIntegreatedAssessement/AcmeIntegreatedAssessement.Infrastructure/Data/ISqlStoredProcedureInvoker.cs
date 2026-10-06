using Microsoft.Data.SqlClient;

namespace AcmeIntegreatedAssessement.Infrastructure.Data
{
    public interface ISqlStoredProcedureInvoker
    {
        Task<int> ExecuteAsync(
            string procedureName,
            Action<SqlParameterCollection>? configureParameters = null,
            CancellationToken cancellationToken = default);

        Task<T?> QuerySingleAsync<T>(
            string procedureName,
            Func<SqlDataReader, T> map,
            Action<SqlParameterCollection>? configureParameters = null,
            CancellationToken cancellationToken = default);

        Task<List<T>> QueryAsync<T>(
            string procedureName,
            Func<SqlDataReader, T> map,
            Action<SqlParameterCollection>? configureParameters = null,
            CancellationToken cancellationToken = default);
    }
}
