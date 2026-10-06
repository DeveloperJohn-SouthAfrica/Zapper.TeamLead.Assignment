using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Application.Common.Models;
using AcmeIntegreatedAssessement.Application.Queries.GetDailyPaymentSummary;
using Moq;
using Xunit;

namespace AcmeIntegreatedAssessement.Application.UnitTests;

public class GetDailyPaymentSummaryQueryHandlerTests
{
    [Fact]
    public async Task Handle_GroupsEntriesIntoSummaries()
    {
        var merchantA = Guid.NewGuid();
        var merchantB = Guid.NewGuid();
        var repositoryMock = new Mock<IDailyPaymentSummaryRepository>();
        repositoryMock.Setup(repo => repo.GetDailySummaryAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DailyPaymentSummaryEntry>
            {
                new(merchantA, "RRN-A", "ZAR", "Approved", 3, 300m),
                new(merchantA, "RRN-A", "ZAR", "Declined", 1, 50m),
                new(merchantA, "RRN-A", "ZAR", "Refunded", 1, 125m),
                new(merchantB, "RRN-B", "USD", "Approved", 2, 420m),
                new(merchantB, "RRN-B", "USD", "Reversed", 1, 75m)
            });

        var handler = new GetDailyPaymentSummaryQueryHandler(repositoryMock.Object);
        var query = new GetDailyPaymentSummaryQuery(DateOnly.FromDateTime(DateTime.UtcNow.Date));

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.SubMerchantSummaries.Count);
        var summaryA = result.SubMerchantSummaries.Single(s => s.SubMerchantId == merchantA);
        Assert.Equal(3, summaryA.ApprovedCount);
        Assert.Equal(300m, summaryA.ApprovedAmount);
        Assert.Equal(1, summaryA.DeclinedCount);
        Assert.Equal(50m, summaryA.DeclinedAmount);
        Assert.Equal(1, summaryA.RefundedCount);
        Assert.Equal(125m, summaryA.RefundedAmount);

        var summaryB = result.SubMerchantSummaries.Single(s => s.SubMerchantId == merchantB);
        Assert.Equal(2, summaryB.ApprovedCount);
        Assert.Equal(420m, summaryB.ApprovedAmount);
        Assert.Equal(1, summaryB.ReversedCount);
        Assert.Equal(75m, summaryB.ReversedAmount);
    }

    [Fact]
    public async Task Handle_IgnoresRowsWhereEventTypeIsUnknown()
    {
        var merchant = Guid.NewGuid();
        var repositoryMock = new Mock<IDailyPaymentSummaryRepository>();
        repositoryMock.Setup(repo => repo.GetDailySummaryAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DailyPaymentSummaryEntry>
            {
                new(merchant, "RRN-Z", "ZAR", "Unexpected", 5, 250m)
            });

        var handler = new GetDailyPaymentSummaryQueryHandler(repositoryMock.Object);
        var query = new GetDailyPaymentSummaryQuery(DateOnly.FromDateTime(DateTime.UtcNow.Date));

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Empty(result.SubMerchantSummaries);
    }
}
