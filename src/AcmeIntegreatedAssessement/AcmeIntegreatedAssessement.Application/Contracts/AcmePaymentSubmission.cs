namespace AcmeIntegreatedAssessement.Application.Contracts;

public sealed record AcmePaymentSubmission(
    Guid TransactionId,
    string OrderNumber,
    long Amount,
    string CurrencyCode,
    string PaymentService,
    string CardToken,
    string CardOnFileScenario,
    string SubMerchantId,
    string MerchantCategoryCode,
    string CardAcceptorName,
    string Email);
