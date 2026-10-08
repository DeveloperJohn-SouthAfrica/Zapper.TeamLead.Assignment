CREATE PROCEDURE [dbo].[GetPaymentBySubMerchantRrn]
    @SubMerchantRrn NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [PaymentId], [AppPaymentId], [Amount], [Currency], [SubMerchantRrn],
           [Status], [AcmeTransactionUuid], [SubMerchantId]
    FROM [dbo].[Payment]
    WHERE [SubMerchantRrn] = @SubMerchantRrn;
END;
