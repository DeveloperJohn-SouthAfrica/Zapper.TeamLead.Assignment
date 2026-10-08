CREATE PROCEDURE [dbo].[GetStoredCardTokenByCustomerId]
    @CustomerId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1) [Id], [StoredCardTokenId], [AcmeToken], [CustomerId], [CustomerEmail]
    FROM [dbo].[StoredCardToken]
    WHERE [CustomerId] = @CustomerId
    ORDER BY [CreatedAt], [Id];
END;
