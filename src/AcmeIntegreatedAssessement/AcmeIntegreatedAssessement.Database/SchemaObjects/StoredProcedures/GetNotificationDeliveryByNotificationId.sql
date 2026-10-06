CREATE PROCEDURE [dbo].[GetNotificationDeliveryByNotificationId]
    @NotificationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NotificationIdParam UNIQUEIDENTIFIER = @NotificationId;

    IF EXISTS (
        SELECT 1
        FROM [dbo].[NotificationDelivery] WITH (NOLOCK)
        WHERE NotificationId = @NotificationIdParam)
    BEGIN
        SELECT 1;
    END
END;
