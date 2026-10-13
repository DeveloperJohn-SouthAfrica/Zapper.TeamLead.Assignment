CREATE PROCEDURE [dbo].[GetSubMerchantByZapperMerchantId]
    @ZapperMerchantId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [ZapperMerchantId], [TradingName], [City], [Region], [Country], [MerchantCategoryCode]
    FROM [dbo].[SubMerchant]
    WHERE [ZapperMerchantId] = @ZapperMerchantId;
END;
