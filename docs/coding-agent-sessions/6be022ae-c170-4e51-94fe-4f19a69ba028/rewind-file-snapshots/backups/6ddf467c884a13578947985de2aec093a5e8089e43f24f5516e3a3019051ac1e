CREATE TABLE [dbo].[NotificationDelivery]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [NotificationId] UNIQUEIDENTIFIER NOT NULL,
    [AppPaymentId] NVARCHAR(100) NOT NULL,
    [PaymentId] UNIQUEIDENTIFIER NOT NULL,
    [NotificationType] NVARCHAR(50) NOT NULL,
    [OccurredAt] DATETIME2 NOT NULL,
    [IsApplied] BIT NOT NULL,
    [RawPayloadJson] NVARCHAR(MAX) NOT NULL
);
