CREATE PROCEDURE [dbo].[ApplyPaymentNotification]
    @NotificationId UNIQUEIDENTIFIER,
    @AppPaymentId NVARCHAR(100),
    @PaymentId UNIQUEIDENTIFIER,
    @NotificationType NVARCHAR(50),
    @OccurredAt DATETIME2,
    @RawPayloadJson NVARCHAR(MAX),
    @ChangeId UNIQUEIDENTIFIER,
    @ChangeIdentifier NVARCHAR(100),
    @EventType NVARCHAR(50),
    @IsApproved BIT,
    @Rrn NVARCHAR(100),
    @Amount DECIMAL(18,2),
    @PaymentStatus NVARCHAR(50) = NULL,
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (
            SELECT 1
            FROM [dbo].[NotificationDelivery] WITH (UPDLOCK, HOLDLOCK)
            WHERE [NotificationId] = @NotificationId)
        BEGIN
            COMMIT TRANSACTION;
            RETURN;
        END;

        DECLARE @LastEventAt DATETIME2;
        SELECT @LastEventAt = MAX([OccurredAt])
        FROM [dbo].[PaymentChange] WITH (UPDLOCK, HOLDLOCK)
        WHERE [PaymentId] = @PaymentId;

        DECLARE @IsApplied BIT =
            CASE WHEN @LastEventAt IS NULL OR @OccurredAt >= @LastEventAt THEN 1 ELSE 0 END;

        INSERT INTO [dbo].[NotificationDelivery] (
            [Id], [NotificationId], [AppPaymentId], [PaymentId], [NotificationType],
            [OccurredAt], [IsApplied], [RawPayloadJson], [CreatedBy])
        VALUES (
            NEWID(), @NotificationId, @AppPaymentId, @PaymentId, @NotificationType,
            @OccurredAt, @IsApplied, @RawPayloadJson, @CreatedBy);

        INSERT INTO [dbo].[PaymentChange] (
            [Id], [PaymentChangeId], [EventType], [IsApproved],
            [AcmeNotificationTransactionUuid], [Rrn], [Amount], [OccurredAt],
            [PaymentId], [CreatedBy])
        VALUES (
            @ChangeId, @ChangeIdentifier, @EventType, @IsApproved,
            @NotificationId, @Rrn, @Amount, @OccurredAt, @PaymentId, @CreatedBy);

        IF @IsApplied = 1 AND @PaymentStatus IS NOT NULL
        BEGIN
            UPDATE [dbo].[Payment]
            SET [Status] = @PaymentStatus,
                [UpdatedAt] = SYSUTCDATETIME(),
                [UpdatedBy] = @CreatedBy
            WHERE [Id] = @PaymentId;
        END;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
