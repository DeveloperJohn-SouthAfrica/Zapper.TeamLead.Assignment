using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;

namespace AcmeIntegreatedAssessement.Infrastructure.Services
{
    /// <inheritdoc />
    public class AcmePayClient : IAcmePayClient
    {
        private readonly HttpClient _httpClient;

        public AcmePayClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SubmitPaymentAsync(string zapperMerchantId,
                                                     string subMerchantRrn,
                                                     string customerId,
                                                     decimal amount,
                                                     string currency,
                                                     string appPaymentId,
                                                     CancellationToken cancellationToken = default)
        {
            var envelope = new
            {
                MessageHeader = new
                {
                    MessageDateTime = DateTimeOffset.UtcNow.ToString("o"),
                    MessageID = Guid.NewGuid(),
                    Channel = "App"
                },
                PaymentRequest = new
                {
                    MerchantID = zapperMerchantId,
                    SubMerchantID = subMerchantRrn,
                    CardOnFileScenario = "CIT",
                    StoredCardToken = customerId,
                    Amount = (int)(amount * 100), // lowest monetary unit
                    CurrencyCode = currency,
                    AppPaymentID = appPaymentId
                }
            };

            using var response = await _httpClient.PostAsJsonAsync("payments", envelope, cancellationToken);
            response.EnsureSuccessStatusCode();

            // Expecting Acme transaction UUID in response body as JSON: { "TransactionID": "..." }
            using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var doc = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken);
            if (doc.RootElement.TryGetProperty("TransactionID", out var tx))
            {
                return tx.GetString()!;
            }
            throw new InvalidOperationException("Missing TransactionID in Acme Pay response");
        }
    }
}
