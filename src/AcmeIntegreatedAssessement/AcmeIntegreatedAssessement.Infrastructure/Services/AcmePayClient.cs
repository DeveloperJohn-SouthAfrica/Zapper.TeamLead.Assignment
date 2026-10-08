using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Application.Contracts;
using Microsoft.Extensions.Configuration;

namespace AcmeIntegreatedAssessement.Infrastructure.Services
{
    public class AcmePayClient : IAcmePayClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null
        };

        private readonly HttpClient _httpClient;
        private readonly Guid _merchantId;

        public AcmePayClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _merchantId = Guid.TryParse(configuration["ACME_PAY_MERCHANT_ID"] ?? configuration["AcmePay:MerchantId"], out var merchantId)
                ? merchantId
                : throw new InvalidOperationException("Configuration 'AcmePay:MerchantId' must be a valid GUID.");
        }

        public async Task<AcmePaymentResult> SubmitPaymentAsync(
            AcmePaymentSubmission payment,
            CancellationToken cancellationToken = default)
        {
            var request = new
            {
                MessageHeader = CreateHeader(),
                PaymentRequest = new
                {
                    MerchantID = _merchantId,
                    TransactionID = payment.TransactionId,
                    payment.OrderNumber,
                    payment.Amount,
                    payment.CurrencyCode,
                    payment.PaymentService,
                    Card = new { Token = payment.CardToken },
                    payment.CardOnFileScenario,
                    SubMerchantID = payment.SubMerchantId,
                    payment.MerchantCategoryCode,
                    payment.CardAcceptorName,
                    payment.Email
                }
            };

            for (var attempt = 0; ; attempt++)
            {
                using var response = await _httpClient.PostAsJsonAsync("/api/payment", request, JsonOptions, cancellationToken);
                if (response.StatusCode == HttpStatusCode.TooManyRequests && attempt < 2)
                {
                    await Task.Delay(GetRetryDelay(response), cancellationToken);
                    continue;
                }

                return await ReadResponseAsync(response, cancellationToken);
            }
        }

        public async Task<AcmePaymentResult?> QueryPaymentAsync(
            Guid transactionId,
            CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(
                $"/apirttq/query/Merchant/{_merchantId:D}?TransactionID={Uri.EscapeDataString(transactionId.ToString("D"))}",
                cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            return await ReadResponseAsync(response, cancellationToken);
        }

        private static object CreateHeader() => new
        {
            MessageDateTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(2)).ToString("yyyy-MM-dd'T'HH:mm:ss.fff"),
            MessageID = Guid.NewGuid(),
            Channel = "App"
        };

        private static TimeSpan GetRetryDelay(HttpResponseMessage response)
        {
            var retryAfter = response.Headers.RetryAfter;
            if (retryAfter?.Delta is { } delta)
            {
                return delta > TimeSpan.Zero ? delta : TimeSpan.FromSeconds(1);
            }

            if (retryAfter?.Date is { } date)
            {
                var delay = date - DateTimeOffset.UtcNow;
                return delay > TimeSpan.Zero ? delay : TimeSpan.FromSeconds(1);
            }

            return TimeSpan.FromSeconds(1);
        }

        private static async Task<AcmePaymentResult> ReadResponseAsync(
            HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            var envelope = await response.Content.ReadFromJsonAsync<PaymentEnvelope>(JsonOptions, cancellationToken)
                ?? throw new InvalidOperationException("Acme Pay returned an empty response.");
            if (!response.IsSuccessStatusCode)
            {
                var error = envelope.PaymentResponse?.ResponseDetail;
                throw new HttpRequestException(
                    $"Acme Pay returned HTTP {(int)response.StatusCode} ({error?.Code ?? "unknown"}): {error?.Description ?? response.ReasonPhrase}",
                    null,
                    response.StatusCode);
            }

            var payment = envelope.PaymentResponse
                ?? throw new InvalidOperationException("Acme Pay response did not contain PaymentResponse.");
            if (!Guid.TryParse(payment.TransactionID, out var transactionId))
            {
                throw new InvalidOperationException("Acme Pay response did not contain a valid PaymentResponse.TransactionID.");
            }

            return new AcmePaymentResult(
                transactionId,
                payment.TransactionStatus ?? "Processing",
                payment.ResponseDetail?.Code,
                payment.ResponseDetail?.Description,
                payment.AuthAmount);
        }

        private sealed class PaymentEnvelope
        {
            public PaymentResponse? PaymentResponse { get; init; }
        }

        private sealed class PaymentResponse
        {
            public string? TransactionID { get; init; }
            public string? TransactionStatus { get; init; }
            public ResponseDetail? ResponseDetail { get; init; }
            public long? AuthAmount { get; init; }
        }

        private sealed class ResponseDetail
        {
            public string? Code { get; init; }
            public string? Description { get; init; }
        }
    }
}
