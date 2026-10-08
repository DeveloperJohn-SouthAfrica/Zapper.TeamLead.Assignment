using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AcmeIntegreatedAssessement.Api.Contracts;
using AcmeIntegreatedAssessement.Api.Services;
using AcmeIntegreatedAssessement.Application.Commands.ProcessAcmeApprovedNotification;
using AcmeIntegreatedAssessement.Application.Commands.ProcessAcmeReversalNotification;
using AcmeIntegreatedAssessement.Application.Commands.SubmitPayment;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Application.Contracts;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Api.Controllers
{
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private const int MaxCallbackBodyBytes = 64 * 1024;
        private static readonly JsonSerializerOptions CallbackJsonOptions = new()
        {
            PropertyNameCaseInsensitive = false
        };

        private readonly ISender _mediator;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IConfiguration _configuration;

        public CommandsController(
            ISender mediator,
            IPaymentRepository paymentRepository,
            IConfiguration configuration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _paymentRepository = paymentRepository;
            _configuration = configuration;
        }

        [HttpPost("api/payments/acme/notifications/approved")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ProcessAcmeApprovedNotification(CancellationToken cancellationToken = default)
        {
            if (!AcmeCallbackAuthentication.IsValidBasicCredentials(
                    Request,
                    GetConfiguration("ACME_ADVICE_USERNAME", "AcmeAdvice:Username"),
                    GetConfiguration("ACME_ADVICE_PASSWORD", "AcmeAdvice:Password")))
            {
                return Unauthorized();
            }

            var rawBody = await ReadCallbackBodyAsync(cancellationToken);
            if (rawBody is null)
            {
                return BadRequest();
            }

            AcmePaymentAdvicePayload? payload;
            try
            {
                payload = JsonSerializer.Deserialize<AcmePaymentAdvicePayload>(rawBody, CallbackJsonOptions);
            }
            catch (JsonException)
            {
                return BadRequest();
            }

            if (payload is null ||
                !Guid.TryParse(payload.TransactionID, out var transactionId) ||
                !Guid.TryParse(payload.MerchantID, out var merchantId) ||
                !Guid.TryParse(GetConfiguration("ACME_PAY_MERCHANT_ID", "AcmePay:MerchantId"), out var configuredMerchantId) ||
                merchantId != configuredMerchantId ||
                string.IsNullOrWhiteSpace(payload.RequestType) ||
                string.IsNullOrWhiteSpace(payload.RetrievalReferenceNumber) ||
                string.IsNullOrWhiteSpace(payload.ResponseCode) ||
                payload.Amount <= 0 ||
                payload.TransactionDateTime == default)
            {
                return BadRequest();
            }

            var payment = await _paymentRepository.GetByAcmeTransactionUuidAsync(transactionId, cancellationToken);
            if (payment is null)
            {
                return NotFound();
            }

            if (payment.SubMerchantRrn != payload.RetrievalReferenceNumber)
            {
                return BadRequest();
            }

            var eventIdSource = string.Join(
                "|",
                transactionId.ToString("D"),
                payload.RequestType,
                payload.TransactionDateTime.ToString("O", CultureInfo.InvariantCulture),
                payload.RetrievalReferenceNumber,
                payload.Amount.ToString(CultureInfo.InvariantCulture),
                payload.ResponseCode);
            var eventId = new Guid(SHA256.HashData(Encoding.UTF8.GetBytes(eventIdSource)).AsSpan(0, 16));
            var occurredAt = ToSast(payload.TransactionDateTime);
            var isApproved =
                (string.Equals(payload.TransactionStatus, "Success", StringComparison.Ordinal) &&
                 string.Equals(payload.ResponseCode, "00", StringComparison.Ordinal)) ||
                (string.Equals(payload.TransactionStatus, "Complete", StringComparison.Ordinal) &&
                 string.Equals(payload.ResponseCode, "COMPL", StringComparison.Ordinal));
            var body = new AcmeApprovedNotificationDto
            {
                EventType = payload.RequestType,
                IsApproved = isApproved,
                AcmeNotificationTransactionUuid = eventId,
                Rrn = payload.RetrievalReferenceNumber,
                Amount = payload.Amount,
                OccurredAt = occurredAt,
                AppPaymentId = payment.AppPaymentId,
                RequestType = payload.RequestType,
                MerchantID = payload.MerchantID!,
                TransactionID = payload.TransactionID!,
                TransactionDateTime = payload.TransactionDateTime,
                OrderNumber = payload.OrderNumber ?? string.Empty,
                ReconID = payload.ReconID ?? string.Empty,
                RetrievalReferenceNumber = payload.RetrievalReferenceNumber,
                SystemsTraceAuditNumber = payload.SystemsTraceAuditNumber ?? string.Empty,
                CurrencyCode = payload.CurrencyCode ?? "ZAR",
                PaymentService = payload.PaymentService ?? string.Empty,
                TransactionStatus = payload.TransactionStatus ?? string.Empty,
                ResponseSource = payload.ResponseSource ?? string.Empty,
                ResponseCode = payload.ResponseCode,
                ResponseDescription = payload.ResponseDescription ?? string.Empty,
                AuthCode = payload.AuthCode ?? string.Empty,
                MaskedCardNumber = payload.MaskedCardNumber ?? string.Empty,
                CommProtocol = payload.CommProtocol ?? string.Empty
            };

            await _mediator.Send(
                new ProcessAcmeApprovedNotificationCommand(
                    payload.RequestType,
                    isApproved,
                    eventId,
                    payload.RetrievalReferenceNumber,
                    payload.Amount,
                    occurredAt,
                    payment.AppPaymentId,
                    body,
                    Encoding.UTF8.GetString(rawBody)),
                cancellationToken);
            return Ok(ApiMessageEnvelope.Create(new MessageAcknowledgement(true), "Web"));
        }

        [HttpPost("api/payments/acme/notifications/reversal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ProcessAcmeReversalNotification(CancellationToken cancellationToken = default)
        {
            var rawBody = await ReadCallbackBodyAsync(cancellationToken);
            if (rawBody is null)
            {
                return BadRequest();
            }

            if (!AcmeCallbackAuthentication.IsValidSignature(
                    rawBody,
                    Request.Headers["x-signature"].ToString(),
                    GetConfiguration("ACME_WEBHOOK_SECRET", "AcmeWebhook:Secret")))
            {
                return Unauthorized();
            }

            AcmeTransactionStatusWebhookPayload? payload;
            try
            {
                payload = JsonSerializer.Deserialize<AcmeTransactionStatusWebhookPayload>(rawBody, CallbackJsonOptions);
            }
            catch (JsonException)
            {
                return BadRequest();
            }

            if (payload is null ||
                !Guid.TryParse(payload.TransactionUUID, out var notificationId) ||
                string.IsNullOrWhiteSpace(payload.Rrn) ||
                payload.TransactionType is not ("Reversal" or "Refund") ||
                string.IsNullOrWhiteSpace(payload.TransactionDateTime) ||
                payload.TransactionAmount <= 0 ||
                string.IsNullOrWhiteSpace(payload.TxResultCode) ||
                !DateTime.TryParse(payload.TransactionDateTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out var transactionDateTime))
            {
                return BadRequest();
            }

            var payment = await _paymentRepository.GetBySubMerchantRrnAsync(payload.Rrn, cancellationToken);
            if (payment is null)
            {
                return NotFound();
            }
            if (!string.Equals(payment.Currency, payload.CurrencyCode ?? "ZAR", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest();
            }

            var occurredAt = ToSast(DateTime.SpecifyKind(transactionDateTime, DateTimeKind.Unspecified));
            var isApplied = payload.IsApproved && string.Equals(payload.TxResultCode, "00", StringComparison.Ordinal);
            var body = new AcmeReversalNotificationDto
            {
                EventType = payload.TransactionType,
                IsApproved = payload.IsApproved,
                AcmeNotificationTransactionUuid = notificationId,
                Rrn = payload.Rrn,
                Amount = payload.TransactionAmount,
                OccurredAt = occurredAt,
                AppPaymentId = payment.AppPaymentId,
                TransactionUUID = payload.TransactionUUID!,
                TransactionType = payload.TransactionType,
                TransactionDateTime = transactionDateTime,
                TransactionAmount = payload.TransactionAmount,
                CurrencyCode = payload.CurrencyCode ?? "ZAR",
                TxResultCode = payload.TxResultCode ?? string.Empty
            };

            await _mediator.Send(
                new ProcessAcmeReversalNotificationCommand(
                    payload.TransactionType,
                    isApplied,
                    notificationId,
                    payload.Rrn,
                    payload.TransactionAmount,
                    occurredAt,
                    payment.AppPaymentId,
                    body,
                    Encoding.UTF8.GetString(rawBody)),
                cancellationToken);
            return Ok(ApiMessageEnvelope.Create(new MessageAcknowledgement(true), "Web"));
        }

        [HttpPost("api/payments")]
        [ProducesResponseType(typeof(MessageEnvelope<PaymentStateResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MessageEnvelope<PaymentStateResponseDto>>> SubmitPayment(
            [FromBody] MessageEnvelope<SubmitPaymentRequestDto> envelope,
            CancellationToken cancellationToken = default)
        {
            if (!ApiMessageEnvelope.IsValidHeader(envelope.MessageHeader) ||
                envelope.Payload is not { } payload ||
                string.IsNullOrWhiteSpace(payload.AppPaymentId) ||
                string.IsNullOrWhiteSpace(payload.ZapperMerchantId) ||
                string.IsNullOrWhiteSpace(payload.CustomerId) ||
                payload.Amount <= 0)
            {
                return BadRequest();
            }

            var command = new SubmitPaymentCommand(
                payload.AppPaymentId,
                payload.Amount,
                string.IsNullOrWhiteSpace(payload.CurrencyCode) ? "ZAR" : payload.CurrencyCode,
                payload.ZapperMerchantId,
                payload.CustomerId,
                string.Empty,
                payload);
            await _mediator.Send(command, cancellationToken);
            var payment = await _paymentRepository.GetByAppPaymentIdAsync(payload.AppPaymentId, cancellationToken)
                ?? throw new InvalidOperationException($"Payment '{payload.AppPaymentId}' could not be read after submission.");
            var response = new PaymentStateResponseDto
            {
                AppPaymentId = payment.AppPaymentId,
                Status = payment.Status,
                Amount = payment.Amount,
                Currency = payment.Currency
            };
            return Ok(ApiMessageEnvelope.Create(response, envelope.MessageHeader!.Channel!));
        }

        private string? GetConfiguration(string environmentVariable, string configurationKey) =>
            _configuration[environmentVariable] ?? _configuration[configurationKey];

        private async Task<byte[]?> ReadCallbackBodyAsync(CancellationToken cancellationToken)
        {
            if (Request.ContentLength > MaxCallbackBodyBytes)
            {
                return null;
            }

            using var body = new MemoryStream();
            var buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = await Request.Body.ReadAsync(buffer, cancellationToken)) > 0)
            {
                if (body.Length + bytesRead > MaxCallbackBodyBytes)
                {
                    return null;
                }

                await body.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            }

            return body.ToArray();
        }

        private static DateTimeOffset ToSast(DateTime dateTime) =>
            new(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), TimeSpan.FromHours(2));
    }
}
