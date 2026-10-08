using System;

namespace AcmeIntegreatedAssessement.Application.Common.Models
{
    public sealed record DailyPaymentSummaryEntry(
        Guid SubMerchantId,
        string Currency,
        string EventType,
        long EventCount,
        long TotalAmount);
}
