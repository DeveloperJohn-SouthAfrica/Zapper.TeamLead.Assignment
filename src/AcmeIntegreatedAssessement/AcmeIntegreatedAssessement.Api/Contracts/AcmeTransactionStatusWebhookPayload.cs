using System.Text.Json.Serialization;

namespace AcmeIntegreatedAssessement.Api.Contracts;

public sealed record AcmeTransactionStatusWebhookPayload
{
    [JsonPropertyName("transactionUUID")]
    public string? TransactionUUID { get; init; }

    [JsonPropertyName("transactionType")]
    public string? TransactionType { get; init; }

    [JsonPropertyName("transactionDateTime")]
    public string? TransactionDateTime { get; init; }

    [JsonPropertyName("rrn")]
    public string? Rrn { get; init; }

    [JsonPropertyName("transactionAmount")]
    public long TransactionAmount { get; init; }

    [JsonPropertyName("currencyCode")]
    public string? CurrencyCode { get; init; }

    [JsonPropertyName("txResultCode")]
    public string? TxResultCode { get; init; }

    [JsonPropertyName("isApproved")]
    public bool IsApproved { get; init; }
}
