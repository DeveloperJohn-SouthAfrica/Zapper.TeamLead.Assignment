using System;
using System.Collections.Generic;

namespace AcmeIntegreatedAssessement.Application.Queries.GetDailyPaymentSummary
{
    public sealed class DailyPaymentSummaryResponse
    {
        public DateOnly ReportDate { get; init; }
        public IReadOnlyList<DailyPaymentSummarySubMerchant> SubMerchantSummaries { get; init; } = Array.Empty<DailyPaymentSummarySubMerchant>();
    }

    public sealed class DailyPaymentSummarySubMerchant
    {
        public Guid SubMerchantId { get; init; }
        public string Currency { get; init; } = string.Empty;
        public long ApprovedCount { get; set; }
        public long ApprovedAmount { get; set; }
        public long DeclinedCount { get; set; }
        public long DeclinedAmount { get; set; }
        public long ReversedCount { get; set; }
        public long ReversedAmount { get; set; }
        public long RefundedCount { get; set; }
        public long RefundedAmount { get; set; }

        public long TotalEvents =>
            ApprovedCount + DeclinedCount + ReversedCount + RefundedCount;

        public long TotalAmount =>
            ApprovedAmount + DeclinedAmount + ReversedAmount + RefundedAmount;
    }
}
