CREATE PROCEDURE [dbo].[GetPaymentByAppPaymentId]
    @AppPaymentId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AppPaymentIdParam NVARCHAR(100) = @AppPaymentId;

    SELECT
        [Id],
        [PaymentId],
        [AppPaymentId],
        [Amount],
        [Currency],
        [SubMerchantRrn],
        [Status],
        [AcmeTransactionUuid],
        [SubMerchantId],
        [CreatedAt],
        [UpdatedAt]
    FROM [dbo].[Payment]
    WHERE [AppPaymentId] = @AppPaymentIdParam;
END;
