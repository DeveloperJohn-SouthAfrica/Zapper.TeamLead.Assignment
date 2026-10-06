CREATE PROCEDURE [dbo].[InsertNotificationDelivery]
    @NotificationId UNIQUEIDENTIFIER,
    @AppPaymentId NVARCHAR(100),
    @PaymentId UNIQUEIDENTIFIER,
    @NotificationType NVARCHAR(50),
    @OccurredAt DATETIME2,
    @IsApplied BIT,
    @RawPayloadJson NVARCHAR(MAX),
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NotificationIdParam UNIQUEIDENTIFIER = @NotificationId;
    DECLARE @AppPaymentIdParam NVARCHAR(100) = @AppPaymentId;
    DECLARE @NotificationTypeParam NVARCHAR(50) = @NotificationType;

    IF EXISTS (
        SELECT 1
        FROM [dbo].[NotificationDelivery] WITH (UPDLOCK, HOLDLOCK)
        WHERE NotificationId = @NotificationIdParam)
    BEGIN
        RETURN;
    END

    INSERT INTO [dbo].[NotificationDelivery] (
        [Id], [NotificationId], [AppPaymentId], [PaymentId], [NotificationType],
        [OccurredAt], [IsApplied], [RawPayloadJson], [CreatedBy])
    VALUES (
        NEWID(), @NotificationIdParam, @AppPaymentIdParam, @PaymentId, @NotificationTypeParam,
        @OccurredAt, @IsApplied, @RawPayloadJson, @CreatedBy);
END;
