CREATE PROCEDURE [dbo].[GetDailyPaymentSummary]
    @ReportDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ReportDateParam DATE = @ReportDate;
    DECLARE @ReportStart DATETIME2 = DATEADD(HOUR, -2, CONVERT(DATETIME2, @ReportDateParam));
    DECLARE @ReportEnd DATETIME2 = DATEADD(DAY, 1, @ReportStart);

    SELECT
        p.SubMerchantId,
        p.Currency,
        pc.EventType,
        COUNT_BIG(*) AS EventCount,
        SUM(pc.Amount) AS TotalAmount
    FROM [dbo].[PaymentChange] pc
    INNER JOIN [dbo].[Payment] p ON p.Id = pc.PaymentId
    WHERE pc.OccurredAt >= @ReportStart
      AND pc.OccurredAt < @ReportEnd
      AND pc.EventType IN ('Approved','Declined','Reversed','Refunded')
    GROUP BY p.SubMerchantId, p.Currency, pc.EventType;
END;
