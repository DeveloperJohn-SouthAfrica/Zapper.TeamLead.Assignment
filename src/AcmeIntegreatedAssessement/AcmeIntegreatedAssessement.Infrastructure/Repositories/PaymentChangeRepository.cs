using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Domain.Entities;
using AcmeIntegreatedAssessement.Infrastructure.Audit;
using AcmeIntegreatedAssessement.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace AcmeIntegreatedAssessement.Infrastructure.Repositories
{
    public class PaymentChangeRepository : IPaymentChangeRepository
    {
        private readonly ISqlStoredProcedureInvoker _invoker;

        public PaymentChangeRepository(ISqlStoredProcedureInvoker invoker)
        {
            _invoker = invoker;
        }

        public async Task AddAsync(PaymentChange change, CancellationToken cancellationToken = default)
        {
            await _invoker.ExecuteAsync(
                "InsertPaymentChange",
                parameters =>
                {
                    AddParameter(parameters, "@PaymentChangeId", change.Id, SqlDbType.UniqueIdentifier);
                    AddParameter(parameters, "@PaymentChangeIdentifier", change.PaymentChangeId, SqlDbType.NVarChar, 100);
                    AddParameter(parameters, "@PaymentId", change.PaymentId, SqlDbType.UniqueIdentifier);
                    AddParameter(parameters, "@EventType", change.EventType, SqlDbType.NVarChar, 50);
                    AddParameter(parameters, "@IsApproved", change.IsApproved, SqlDbType.Bit);
                    AddParameter(parameters, "@AcmeNotificationTransactionUuid", TryParseGuid(change.AcmeNotificationTransactionUuid), SqlDbType.UniqueIdentifier);
                    AddParameter(parameters, "@Rrn", change.Rrn, SqlDbType.NVarChar, 100);
                    AddParameter(parameters, "@Amount", change.Amount, SqlDbType.Decimal, precision: 18, scale: 2);
                    AddParameter(parameters, "@OccurredAt", change.OccurredAt, SqlDbType.DateTime2);
                    AddParameter(parameters, "@CreatedBy", AuditActor.System, SqlDbType.NVarChar, 100);
                },
                cancellationToken);
        }

        private static Guid? TryParseGuid(string? value)
        {
            return Guid.TryParse(value, out var guid) ? guid : null;
        }

        private static void AddParameter(
            SqlParameterCollection parameters,
            string name,
            object? value,
            SqlDbType dbType,
            int? size = null,
            byte? precision = null,
            byte? scale = null)
        {
            var parameter = parameters.Add(name, dbType);
            if (size.HasValue)
            {
                parameter.Size = size.Value;
            }
            if (precision.HasValue)
            {
                parameter.Precision = precision.Value;
            }
            if (scale.HasValue)
            {
                parameter.Scale = scale.Value;
            }
            parameter.Value = value ?? DBNull.Value;
        }
    }
}
