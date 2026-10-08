CREATE PROCEDURE [dbo].[GetPaymentByAcmeTransactionUuid]
    @AcmeTransactionUuid UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [PaymentId], [AppPaymentId], [Amount], [Currency], [SubMerchantRrn],
           [Status], [AcmeTransactionUuid], [SubMerchantId]
    FROM [dbo].[Payment]
    WHERE [AcmeTransactionUuid] = @AcmeTransactionUuid;
END;
