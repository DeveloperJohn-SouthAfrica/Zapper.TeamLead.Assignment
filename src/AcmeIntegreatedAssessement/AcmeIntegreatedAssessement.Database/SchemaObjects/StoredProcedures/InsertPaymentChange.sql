CREATE PROCEDURE [dbo].[InsertPaymentChange]
    @PaymentChangeId UNIQUEIDENTIFIER,
    @PaymentChangeIdentifier NVARCHAR(100),
    @PaymentId UNIQUEIDENTIFIER,
    @EventType NVARCHAR(50),
    @IsApproved BIT,
    @AcmeNotificationTransactionUuid UNIQUEIDENTIFIER,
    @Rrn NVARCHAR(100),
    @Amount DECIMAL(18,2),
    @OccurredAt DATETIME2,
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EventTypeParam NVARCHAR(50) = @EventType;
    DECLARE @RrnParam NVARCHAR(100) = @Rrn;

    INSERT INTO [dbo].[PaymentChange] (
        [Id], [PaymentChangeId], [EventType], [IsApproved],
        [AcmeNotificationTransactionUuid], [Rrn], [Amount], [OccurredAt],
        [PaymentId], [CreatedBy])
    VALUES (
        @PaymentChangeId, @PaymentChangeIdentifier, @EventTypeParam, @IsApproved,
        @AcmeNotificationTransactionUuid, @RrnParam, @Amount, @OccurredAt,
        @PaymentId, @CreatedBy);
END;
