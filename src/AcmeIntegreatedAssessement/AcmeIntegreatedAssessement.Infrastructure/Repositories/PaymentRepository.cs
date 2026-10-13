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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ISqlStoredProcedureInvoker _invoker;

        public PaymentRepository(ISqlStoredProcedureInvoker invoker)
        {
            _invoker = invoker;
        }

        public async Task<Payment?> GetByAppPaymentIdAsync(string appPaymentId, CancellationToken cancellationToken = default)
        {
            return await _invoker.QuerySingleAsync(
                "GetPaymentByAppPaymentId",
                MapPayment,
                parameters =>
                {
                    AddParameter(parameters, "@AppPaymentId", appPaymentId, SqlDbType.NVarChar, 100);
                },
                cancellationToken);
        }

        public async Task<Payment?> GetByAcmeTransactionUuidAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            return await _invoker.QuerySingleAsync(
                "GetPaymentByAcmeTransactionUuid",
                MapPayment,
                parameters => AddParameter(parameters, "@AcmeTransactionUuid", transactionId, SqlDbType.UniqueIdentifier),
                cancellationToken);
        }

        public async Task<Payment?> GetBySubMerchantRrnAsync(string rrn, CancellationToken cancellationToken = default)
        {
            return await _invoker.QuerySingleAsync(
                "GetPaymentBySubMerchantRrn",
                MapPayment,
                parameters => AddParameter(parameters, "@SubMerchantRrn", rrn, SqlDbType.NVarChar, 100),
                cancellationToken);
        }

        public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            await _invoker.ExecuteAsync(
                "InsertPayment",
                parameters =>
                {
                    AddParameter(parameters, "@PaymentId", payment.Id, SqlDbType.UniqueIdentifier);
                    AddParameter(parameters, "@PaymentIdentifier", payment.PaymentId, SqlDbType.NVarChar, 100);
                    AddParameter(parameters, "@AppPaymentId", payment.AppPaymentId, SqlDbType.NVarChar, 100);
                    AddParameter(parameters, "@Amount", payment.Amount, SqlDbType.BigInt);
                    AddParameter(parameters, "@Currency", payment.Currency, SqlDbType.NVarChar, 3);
                    AddParameter(parameters, "@SubMerchantRrn", payment.SubMerchantRrn, SqlDbType.NVarChar, 100);
                    AddParameter(parameters, "@Status", payment.Status, SqlDbType.NVarChar, 50);
                    AddParameter(parameters, "@AcmeTransactionUuid", TryParseGuid(payment.AcmeTransactionUuid), SqlDbType.UniqueIdentifier);
                    AddParameter(parameters, "@SubMerchantId", payment.SubMerchantId, SqlDbType.UniqueIdentifier);
                    AddParameter(parameters, "@CreatedBy", AuditActor.System, SqlDbType.NVarChar, 100);
                    AddParameter(parameters, "@UpdatedBy", AuditActor.System, SqlDbType.NVarChar, 100);
                },
                cancellationToken);
        }

        public async Task ApplyNotificationAsync(
            NotificationDelivery delivery,
            PaymentChange change,
            string? paymentStatus,
            CancellationToken cancellationToken = default)
        {
            await _invoker.ExecuteAsync(
                "ApplyPaymentNotification",
                parameters =>
                {
                    AddParameter(parameters, "@NotificationId", Guid.Parse(delivery.NotificationId), SqlDbType.UniqueIdentifier);
                    AddParameter(parameters, "@AppPaymentId", delivery.AppPaymentId, SqlDbType.NVarChar, 100);
                    AddParameter(parameters, "@PaymentId", change.PaymentId, SqlDbType.UniqueIdentifier);
                    AddParameter(parameters, "@NotificationType", delivery.NotificationType, SqlDbType.NVarChar, 50);
                    AddParameter(parameters, "@OccurredAt", delivery.OccurredAt, SqlDbType.DateTime2);
                    AddParameter(parameters, "@RawPayloadJson", delivery.RawPayloadJson, SqlDbType.NVarChar, size: -1);
                    AddParameter(parameters, "@ChangeId", change.Id, SqlDbType.UniqueIdentifier);
                    AddParameter(parameters, "@ChangeIdentifier", change.PaymentChangeId, SqlDbType.NVarChar, 100);
                    AddParameter(parameters, "@EventType", change.EventType, SqlDbType.NVarChar, 50);
                    AddParameter(parameters, "@IsApproved", change.IsApproved, SqlDbType.Bit);
                    AddParameter(parameters, "@Rrn", change.Rrn, SqlDbType.NVarChar, 100);
                    AddParameter(parameters, "@Amount", change.Amount, SqlDbType.BigInt);
                    AddParameter(parameters, "@PaymentStatus", paymentStatus, SqlDbType.NVarChar, 50);
                    AddParameter(parameters, "@CreatedBy", AuditActor.System, SqlDbType.NVarChar, 100);
                },
                cancellationToken);
        }

        public async Task UpdateStatusAsync(string appPaymentId, string status, CancellationToken cancellationToken = default)
        {
            await _invoker.ExecuteAsync(
                "UpdatePaymentStatus",
                parameters =>
                {
                    AddParameter(parameters, "@AppPaymentId", appPaymentId, SqlDbType.NVarChar, 100);
                    AddParameter(parameters, "@Status", status, SqlDbType.NVarChar, 50);
                    AddParameter(parameters, "@UpdatedBy", AuditActor.System, SqlDbType.NVarChar, 100);
                },
                cancellationToken);
        }

        private static Payment MapPayment(SqlDataReader reader)
        {
            var acmeOrdinal = reader.GetOrdinal("AcmeTransactionUuid");
            return new Payment
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                PaymentId = reader.GetString(reader.GetOrdinal("PaymentId")),
                AppPaymentId = reader.GetString(reader.GetOrdinal("AppPaymentId")),
                Amount = reader.GetInt64(reader.GetOrdinal("Amount")),
                Currency = reader.GetString(reader.GetOrdinal("Currency")),
                SubMerchantRrn = reader.GetString(reader.GetOrdinal("SubMerchantRrn")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                AcmeTransactionUuid = reader.IsDBNull(acmeOrdinal) ? string.Empty : reader.GetGuid(acmeOrdinal).ToString(),
                SubMerchantId = reader.GetGuid(reader.GetOrdinal("SubMerchantId"))
            };
        }

        private static Guid? TryParseGuid(string? value)
        {
            if (Guid.TryParse(value, out var guid))
            {
                return guid;
            }
            return null;
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
