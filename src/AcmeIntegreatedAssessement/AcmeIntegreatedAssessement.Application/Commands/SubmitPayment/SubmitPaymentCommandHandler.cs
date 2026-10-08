using System.Net;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Application.Contracts;
using AcmeIntegreatedAssessement.Domain.Entities;
using AcmeIntegreatedAssessement.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AcmeIntegreatedAssessement.Application.Commands.SubmitPayment
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SubmitPaymentCommandHandler : IRequestHandler<SubmitPaymentCommand>
    {
        private readonly ISubMerchantRepository _subMerchantRepository;
        private readonly IStoredCardTokenRepository _tokenRepository;
        private readonly IAcmePayClient _acmeClient;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<SubmitPaymentCommandHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public SubmitPaymentCommandHandler(
            ISubMerchantRepository subMerchantRepository,
            IStoredCardTokenRepository tokenRepository,
            IAcmePayClient acmeClient,
            IPaymentRepository paymentRepository,
            ILogger<SubmitPaymentCommandHandler> logger)
        {
            _subMerchantRepository = subMerchantRepository;
            _tokenRepository = tokenRepository;
            _acmeClient = acmeClient;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(SubmitPaymentCommand request, CancellationToken cancellationToken)
        {
            if (await _paymentRepository.GetByAppPaymentIdAsync(request.AppPaymentId, cancellationToken) is not null)
            {
                return;
            }

            var subMerchantId = request.Request.ZapperMerchantId;
            var subMerchant = await _subMerchantRepository.GetByZapperMerchantIdAsync(subMerchantId, cancellationToken)
                ?? throw new InvalidOperationException($"Sub-merchant '{subMerchantId}' not found.");
            var customerId = request.Request.CustomerId;
            var card = await _tokenRepository.GetByCustomerIdAsync(customerId, cancellationToken)
                ?? throw new InvalidOperationException($"Stored card for customer '{customerId}' not found.");

            var candidateTransactionId = Guid.NewGuid();
            var candidateOrderNumber = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
            await _paymentRepository.AddAsync(new Payment
            {
                Id = Guid.NewGuid(),
                PaymentId = candidateTransactionId.ToString("D"),
                AppPaymentId = request.AppPaymentId,
                Amount = request.Request.Amount,
                Currency = GetCurrencyCode(request.Request.CurrencyCode, request.Currency),
                SubMerchantRrn = candidateOrderNumber,
                Status = "Processing",
                AcmeTransactionUuid = candidateTransactionId.ToString("D"),
                SubMerchantId = subMerchant.Id
            }, cancellationToken);

            var payment = await _paymentRepository.GetByAppPaymentIdAsync(request.AppPaymentId, cancellationToken)
                ?? throw new InvalidOperationException($"Payment '{request.AppPaymentId}' could not be persisted.");
            if (!Guid.TryParse(payment.AcmeTransactionUuid, out var transactionId))
            {
                throw new InvalidOperationException($"Payment '{request.AppPaymentId}' has an invalid Acme transaction ID.");
            }

            var submission = new AcmePaymentSubmission(
                transactionId,
                payment.SubMerchantRrn,
                checked((long)payment.Amount),
                payment.Currency,
                "CardNotPresent",
                card.AcmeToken,
                request.Request.CardOnFileScenario,
                subMerchant.ZapperMerchantId,
                subMerchant.MerchantCategoryCode,
                BuildCardAcceptorName(subMerchant),
                card.CustomerEmail);

            try
            {
                var result = await _acmeClient.SubmitPaymentAsync(submission, cancellationToken);
                await ApplyAcmeResultAsync(payment.AppPaymentId, transactionId, result, cancellationToken);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                await ReconcileAmbiguousSubmissionAsync(payment.AppPaymentId, transactionId, cancellationToken);
            }
            catch (HttpRequestException exception) when (
                exception.StatusCode == HttpStatusCode.TooManyRequests ||
                exception.StatusCode is { } statusCode && (int)statusCode >= 500)
            {
                _logger.LogWarning(exception, "Acme payment submission is ambiguous for app payment {AppPaymentId}; reconciling by transaction ID.", payment.AppPaymentId);
                await ReconcileAmbiguousSubmissionAsync(payment.AppPaymentId, transactionId, cancellationToken);
            }
        }

        private async Task ReconcileAmbiguousSubmissionAsync(
            string appPaymentId,
            Guid transactionId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _acmeClient.QueryPaymentAsync(transactionId, cancellationToken);
                if (result is not null)
                {
                    await ApplyAcmeResultAsync(appPaymentId, transactionId, result, cancellationToken);
                }
            }
            catch (Exception exception) when (exception is HttpRequestException or OperationCanceledException)
            {
                _logger.LogWarning(exception, "RTTQ could not resolve Acme transaction {TransactionId}; payment remains Processing.", transactionId);
            }
        }

        private async Task ApplyAcmeResultAsync(
            string appPaymentId,
            Guid transactionId,
            AcmePaymentResult result,
            CancellationToken cancellationToken)
        {
            if (result.TransactionId != transactionId)
            {
                throw new InvalidOperationException("Acme returned a transaction ID that does not match the submitted payment.");
            }

            if (string.Equals(result.TransactionStatus, "Processing", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(result.ResponseCode, "PEND", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var status = string.Equals(result.ResponseCode, "00", StringComparison.OrdinalIgnoreCase)
                ? "Success"
                : string.Equals(result.ResponseCode, "COMPL", StringComparison.OrdinalIgnoreCase)
                    ? "Complete"
                    : "Failure";
            await _paymentRepository.UpdateStatusAsync(appPaymentId, status, cancellationToken);
        }

        private static string GetCurrencyCode(string? requestCurrencyCode, string commandCurrency) =>
            string.IsNullOrWhiteSpace(requestCurrencyCode)
                ? string.IsNullOrWhiteSpace(commandCurrency) ? "ZAR" : commandCurrency
                : requestCurrencyCode;

        private static string BuildCardAcceptorName(SubMerchant subMerchant)
        {
            var name = $"ZAP*{subMerchant.TradingName}".ToUpperInvariant();
            var city = subMerchant.City.ToUpperInvariant();
            var region = subMerchant.Region.Trim().ToUpperInvariant() switch
            {
                "WESTERN CAPE" => "WC",
                "EASTERN CAPE" => "EC",
                "NORTHERN CAPE" => "NC",
                "FREE STATE" => "FS",
                "KWAZULU-NATAL" => "KZ",
                "NORTH WEST" => "NW",
                "GAUTENG" => "GP",
                "MPUMALANGA" => "MP",
                "LIMPOPO" => "LP",
                var code when code.Length == 2 => code,
                _ => throw new InvalidOperationException($"Region '{subMerchant.Region}' has no two-character region code.")
            };
            var country = subMerchant.Country.Trim().ToUpperInvariant() switch
            {
                "SOUTH AFRICA" => "ZA",
                var code when code.Length == 2 => code,
                _ => throw new InvalidOperationException($"Country '{subMerchant.Country}' has no two-character country code.")
            };

            return $"{name[..Math.Min(name.Length, 23)].PadRight(23)}{city[..Math.Min(city.Length, 13)].PadRight(13)}{region}{country}";
        }
    }
}
