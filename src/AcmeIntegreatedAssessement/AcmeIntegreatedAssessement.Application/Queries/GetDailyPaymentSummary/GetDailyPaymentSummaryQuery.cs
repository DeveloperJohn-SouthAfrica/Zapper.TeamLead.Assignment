using System;
using MediatR;

namespace AcmeIntegreatedAssessement.Application.Queries.GetDailyPaymentSummary
{
    public sealed record GetDailyPaymentSummaryQuery(DateOnly ReportDate) : IRequest<DailyPaymentSummaryResponse>;
}
