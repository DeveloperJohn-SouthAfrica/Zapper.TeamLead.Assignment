CREATE PROCEDURE [dbo].[UpdatePaymentStatus]
    @AppPaymentId NVARCHAR(100),
    @Status NVARCHAR(50),
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AppPaymentIdParam NVARCHAR(100) = @AppPaymentId;
    DECLARE @StatusParam NVARCHAR(50) = @Status;

    UPDATE [dbo].[Payment]
    SET [Status] = @StatusParam,
        [UpdatedAt] = SYSUTCDATETIME(),
        [UpdatedBy] = @UpdatedBy
    WHERE [AppPaymentId] = @AppPaymentIdParam
      AND [Status] = 'Processing';
END;
