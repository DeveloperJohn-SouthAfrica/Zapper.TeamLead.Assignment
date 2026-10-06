using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Application.Common.Models;
using AcmeIntegreatedAssessement.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace AcmeIntegreatedAssessement.Infrastructure.Repositories
{
    public class DailyPaymentSummaryRepository : IDailyPaymentSummaryRepository
    {
        private readonly ISqlStoredProcedureInvoker _invoker;

        public DailyPaymentSummaryRepository(ISqlStoredProcedureInvoker invoker)
        {
            _invoker = invoker;
        }

        public Task<List<DailyPaymentSummaryEntry>> GetDailySummaryAsync(DateTime reportDate, CancellationToken cancellationToken = default)
        {
            return _invoker.QueryAsync(
                "GetDailyPaymentSummary",
                Map,
                parameters =>
                {
                    var parameter = parameters.Add("@ReportDate", SqlDbType.Date);
                    parameter.Value = reportDate.Date;
                },
                cancellationToken);
        }

        private static DailyPaymentSummaryEntry Map(SqlDataReader reader)
        {
            var subMerchantId = reader.GetGuid(reader.GetOrdinal("SubMerchantId"));
            var rrn = reader.GetString(reader.GetOrdinal("SubMerchantRrn"));
            var currency = reader.GetString(reader.GetOrdinal("Currency"));
            var eventType = reader.GetString(reader.GetOrdinal("EventType"));
            var eventCount = reader.GetInt64(reader.GetOrdinal("EventCount"));
            var totalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"));

            return new DailyPaymentSummaryEntry(
                subMerchantId,
                rrn,
                currency,
                eventType,
                eventCount,
                totalAmount);
        }
    }
}
