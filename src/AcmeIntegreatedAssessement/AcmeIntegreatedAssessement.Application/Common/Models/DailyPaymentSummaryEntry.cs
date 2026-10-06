using System;

namespace AcmeIntegreatedAssessement.Application.Common.Models
{
    public sealed record DailyPaymentSummaryEntry(
        Guid SubMerchantId,
        string SubMerchantRrn,
        string Currency,
        string EventType,
        long EventCount,
        decimal TotalAmount);
}
