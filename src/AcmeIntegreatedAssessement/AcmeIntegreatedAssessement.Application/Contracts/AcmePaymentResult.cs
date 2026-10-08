namespace AcmeIntegreatedAssessement.Application.Contracts;

public sealed record AcmePaymentResult(
    Guid TransactionId,
    string TransactionStatus,
    string? ResponseCode,
    string? ResponseDescription,
    long? AuthAmount);
