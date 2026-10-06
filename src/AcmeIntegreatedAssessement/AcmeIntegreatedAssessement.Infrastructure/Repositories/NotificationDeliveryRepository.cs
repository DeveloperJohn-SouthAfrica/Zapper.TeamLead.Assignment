using System;
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
    public class NotificationDeliveryRepository : INotificationDeliveryRepository
    {
        private readonly ISqlStoredProcedureInvoker _invoker;

        public NotificationDeliveryRepository(ISqlStoredProcedureInvoker invoker)
        {
            _invoker = invoker;
        }

        public async Task<bool> ExistsAsync(string notificationId, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(notificationId, out var guid))
            {
                return false;
            }

            var result = await _invoker.QuerySingleAsync<int?>(
                "GetNotificationDeliveryByNotificationId",
                reader => reader.GetInt32(0),
                parameters =>
                {
                    AddParameter(parameters, "@NotificationId", guid, SqlDbType.UniqueIdentifier);
                },
                cancellationToken);
            return result.HasValue;
        }

        public async Task AddAsync(NotificationDelivery delivery, CancellationToken cancellationToken = default)
        {
            await _invoker.ExecuteAsync(
                "InsertNotificationDelivery",
                parameters =>
                {
                    AddParameter(parameters, "@NotificationId", ParseGuid(delivery.NotificationId), SqlDbType.UniqueIdentifier);
                    AddParameter(parameters, "@AppPaymentId", delivery.AppPaymentId, SqlDbType.NVarChar, 100);
                    AddParameter(parameters, "@PaymentId", ParseGuid(delivery.PaymentId), SqlDbType.UniqueIdentifier);
                    AddParameter(parameters, "@NotificationType", delivery.NotificationType, SqlDbType.NVarChar, 50);
                    AddParameter(parameters, "@OccurredAt", delivery.OccurredAt, SqlDbType.DateTime2);
                    AddParameter(parameters, "@IsApplied", delivery.IsApplied, SqlDbType.Bit);
                    AddParameter(parameters, "@RawPayloadJson", delivery.RawPayloadJson, SqlDbType.NVarChar, size: -1);
                    AddParameter(parameters, "@CreatedBy", AuditActor.System, SqlDbType.NVarChar, 100);
                },
                cancellationToken);
        }

        private static Guid ParseGuid(string? value)
        {
            if (Guid.TryParse(value, out var guid))
            {
                return guid;
            }

            throw new InvalidOperationException($"Value '{value}' is not a valid Guid.");
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
