CREATE PROCEDURE [dbo].[SeedTestData]
    @Environment NVARCHAR(50) = 'Development'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Env NVARCHAR(50) = @Environment;
    IF @Env NOT IN ('Development', 'Staging')
    BEGIN
        RAISERROR('Invalid environment. Please specify either ''Development'' or ''Staging''.', 16, 1);
        RETURN;
    END

    MERGE INTO [dbo].[SubMerchant] AS target
    USING (
        VALUES
            ('a3c1b5b2-0f70-462e-b0c1-521a5c4c5e7d', 'ZAP123', 'Zapper Retail', 'Cape Town', 'Western Cape', 'South Africa', '5311'),
            ('b5f2a4c3-4d39-4323-9f35-2f9c5a37a3b1', 'ZAP456', 'Zapper Wholesale', 'Johannesburg', 'Gauteng', 'South Africa', '5311')
    ) AS source (Id, ZapperMerchantId, TradingName, City, Region, Country, MerchantCategoryCode)
    ON target.ZapperMerchantId = source.ZapperMerchantId
    WHEN MATCHED THEN
        UPDATE SET
            TradingName = source.TradingName,
            City = source.City,
            Region = source.Region,
            Country = source.Country,
            MerchantCategoryCode = source.MerchantCategoryCode,
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Id, ZapperMerchantId, TradingName, City, Region, Country, MerchantCategoryCode, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
        VALUES (source.Id, source.ZapperMerchantId, source.TradingName, source.City, source.Region, source.Country, source.MerchantCategoryCode, SYSUTCDATETIME(), SYSUTCDATETIME(), 'seed', 'seed');

    MERGE INTO [dbo].[StoredCardToken] AS target
    USING (
        VALUES
            ('4bbd89a1-1bcd-40af-8d54-0d5e3b5ed5c2', 'TOKEN-001', 'ACME-001', 'cust-1', 'cust1@example.com', NULL),
            ('45c752c9-8aee-4db1-afe4-0b4c99ff5d21', 'TOKEN-002', 'ACME-002', 'cust-2', 'cust2@example.com', NULL)
    ) AS source (Id, StoredCardTokenId, AcmeToken, CustomerId, CustomerEmail, PaymentId)
    ON target.StoredCardTokenId = source.StoredCardTokenId
    WHEN MATCHED THEN
        UPDATE SET
            AcmeToken = source.AcmeToken,
            CustomerEmail = source.CustomerEmail,
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Id, StoredCardTokenId, AcmeToken, CustomerId, CustomerEmail, PaymentId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
        VALUES (source.Id, source.StoredCardTokenId, source.AcmeToken, source.CustomerId, source.CustomerEmail, source.PaymentId, SYSUTCDATETIME(), SYSUTCDATETIME(), 'seed', 'seed');
END;
