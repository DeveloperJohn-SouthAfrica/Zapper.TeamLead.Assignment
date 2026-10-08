namespace AcmeIntegreatedAssessement.Api.Contracts;

public sealed record AcmePaymentAdvicePayload
{
    public string? RequestType { get; init; }
    public string? MerchantID { get; init; }
    public string? TransactionID { get; init; }
    public DateTime TransactionDateTime { get; init; }
    public string? OrderNumber { get; init; }
    public string? ReconID { get; init; }
    public string? RetrievalReferenceNumber { get; init; }
    public string? SystemsTraceAuditNumber { get; init; }
    public long Amount { get; init; }
    public string? CurrencyCode { get; init; }
    public string? PaymentService { get; init; }
    public string? TransactionStatus { get; init; }
    public string? ResponseSource { get; init; }
    public string? ResponseCode { get; init; }
    public string? ResponseDescription { get; init; }
    public string? AuthCode { get; init; }
    public string? MaskedCardNumber { get; init; }
    public string? CommProtocol { get; init; }
}
