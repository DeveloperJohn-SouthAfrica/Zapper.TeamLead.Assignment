CREATE TABLE [dbo].[PaymentChange]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [PaymentChangeId] NVARCHAR(100) NOT NULL,
    [EventType] NVARCHAR(50) NOT NULL,
    [IsApproved] BIT NULL,
    [AcmeNotificationTransactionUuid] UNIQUEIDENTIFIER NULL,
    [Rrn] NVARCHAR(100) NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [OccurredAt] DATETIME2 NOT NULL,
    [PaymentId] UNIQUEIDENTIFIER NOT NULL
);
