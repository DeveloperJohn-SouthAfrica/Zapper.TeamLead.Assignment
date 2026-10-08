using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Application.Common.Models;
using MediatR;

namespace AcmeIntegreatedAssessement.Application.Queries.GetDailyPaymentSummary
{
    public sealed class GetDailyPaymentSummaryQueryHandler : IRequestHandler<GetDailyPaymentSummaryQuery, DailyPaymentSummaryResponse>
    {
        private readonly IDailyPaymentSummaryRepository _repository;

        public GetDailyPaymentSummaryQueryHandler(IDailyPaymentSummaryRepository repository)
        {
            _repository = repository;
        }

        public async Task<DailyPaymentSummaryResponse> Handle(GetDailyPaymentSummaryQuery request, CancellationToken cancellationToken)
        {
            var rows = await _repository.GetDailySummaryAsync(request.ReportDate.ToDateTime(TimeOnly.MinValue), cancellationToken);
            var summaries = rows
                .GroupBy(row => (row.SubMerchantId, row.Currency))
                .Select(group =>
                {
                    var summary = new DailyPaymentSummarySubMerchant
                    {
                        SubMerchantId = group.Key.SubMerchantId,
                        Currency = group.Key.Currency
                    };
                    foreach (var entry in group)
                    {
                        ApplyEntry(summary, entry);
                    }
                    return summary;
                })
                .Where(summary => summary.TotalEvents > 0)
                .ToList();

            return new DailyPaymentSummaryResponse
            {
                ReportDate = request.ReportDate,
                SubMerchantSummaries = summaries
            };
        }

        private static void ApplyEntry(DailyPaymentSummarySubMerchant summary, DailyPaymentSummaryEntry entry)
        {
            var eventType = entry.EventType?.Trim();
            if (string.Equals(eventType, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                summary.ApprovedCount = entry.EventCount;
                summary.ApprovedAmount = entry.TotalAmount;
            }
            else if (string.Equals(eventType, "Declined", StringComparison.OrdinalIgnoreCase))
            {
                summary.DeclinedCount = entry.EventCount;
                summary.DeclinedAmount = entry.TotalAmount;
            }
            else if (string.Equals(eventType, "Reversed", StringComparison.OrdinalIgnoreCase))
            {
                summary.ReversedCount = entry.EventCount;
                summary.ReversedAmount = entry.TotalAmount;
            }
            else if (string.Equals(eventType, "Refunded", StringComparison.OrdinalIgnoreCase))
            {
                summary.RefundedCount = entry.EventCount;
                summary.RefundedAmount = entry.TotalAmount;
            }
        }
    }
}
