using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Common.Models;

namespace AcmeIntegreatedAssessement.Application.Common.Interfaces
{
    public interface IDailyPaymentSummaryRepository
    {
        Task<List<DailyPaymentSummaryEntry>> GetDailySummaryAsync(DateTime reportDate, CancellationToken cancellationToken = default);
    }
}
