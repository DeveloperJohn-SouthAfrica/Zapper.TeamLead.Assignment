CREATE TABLE [dbo].[NotificationDelivery]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [NotificationId] UNIQUEIDENTIFIER NOT NULL,
    [AppPaymentId] NVARCHAR(100) NOT NULL,
    [PaymentId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_NotificationDelivery_Payment REFERENCES [dbo].[Payment](Id),
    [NotificationType] NVARCHAR(50) NOT NULL,
    [OccurredAt] DATETIME2 NOT NULL,
    [IsApplied] BIT NOT NULL,
    [RawPayloadJson] NVARCHAR(MAX) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_NotificationDelivery_CreatedAt DEFAULT SYSUTCDATETIME(),
    [CreatedBy] NVARCHAR(100) NOT NULL
);
