CREATE PROCEDURE [dbo].[InsertPayment]
    @PaymentId UNIQUEIDENTIFIER,
    @PaymentIdentifier NVARCHAR(100),
    @AppPaymentId NVARCHAR(100),
    @Amount BIGINT,
    @Currency NVARCHAR(3),
    @SubMerchantRrn NVARCHAR(100),
    @Status NVARCHAR(50),
    @AcmeTransactionUuid UNIQUEIDENTIFIER = NULL,
    @SubMerchantId UNIQUEIDENTIFIER,
    @CreatedBy NVARCHAR(100),
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AppPaymentIdParam NVARCHAR(100) = @AppPaymentId;
    DECLARE @CurrencyParam NVARCHAR(3) = @Currency;
    DECLARE @SubMerchantRrnParam NVARCHAR(100) = @SubMerchantRrn;
    DECLARE @StatusParam NVARCHAR(50) = @Status;

    IF EXISTS (
        SELECT 1
        FROM [dbo].[Payment] WITH (UPDLOCK, HOLDLOCK)
        WHERE AppPaymentId = @AppPaymentIdParam)
    BEGIN
        RETURN;
    END

    INSERT INTO [dbo].[Payment] (
        [Id], [PaymentId], [AppPaymentId], [Amount], [Currency], [SubMerchantRrn],
        [Status], [AcmeTransactionUuid], [SubMerchantId], [CreatedBy], [UpdatedBy])
    VALUES (
        @PaymentId, @PaymentIdentifier, @AppPaymentIdParam, @Amount, @CurrencyParam, @SubMerchantRrnParam,
        @StatusParam, @AcmeTransactionUuid, @SubMerchantId, @CreatedBy, @UpdatedBy);
END;
