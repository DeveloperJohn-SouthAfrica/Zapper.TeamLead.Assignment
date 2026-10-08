using System.Net;
using System.Text;
using System.Text.Json;
using AcmeIntegreatedAssessement.Api.Services;
using AcmeIntegreatedAssessement.Application.Commands.ProcessAcmeApprovedNotification;
using AcmeIntegreatedAssessement.Application.Common.Interfaces;
using AcmeIntegreatedAssessement.Application.Contracts;
using AcmeIntegreatedAssessement.Domain.Entities;
using AcmeIntegreatedAssessement.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace AcmeIntegreatedAssessement.Application.UnitTests;

public class AcmeContractTests
{
    private static readonly Guid MerchantId = Guid.Parse("8B67BF88-BB8D-4EB5-ADFC-0F5C97C7ED67");
    private static readonly Guid TransactionId = Guid.Parse("2E0E1B85-BF91-45A9-B038-282E00036919");

    [Fact]
    public async Task SubmitPaymentAsync_SendsAppendixAEnvelopeAndIgnoresUnknownResponseFields()
    {
        HttpRequestMessage? capturedRequest = null;
        using var httpClient = CreateClient(request =>
        {
            capturedRequest = request;
            return JsonResponse($$"""
                {
                  "MessageHeader": { "MessageID": "{{Guid.NewGuid()}}", "FutureHeader": true },
                  "PaymentResponse": {
                    "TransactionID": "{{TransactionId}}",
                    "TransactionStatus": "Success",
                    "AuthAmount": 1999,
                    "ResponseDetail": { "Code": "00", "Description": "Approved", "FutureDetail": "ignored" },
                    "FutureResponseField": "ignored"
                  }
                }
                """);
        });
        var client = CreateAcmeClient(httpClient);

        var result = await client.SubmitPaymentAsync(CreateSubmission());

        Assert.Equal(TransactionId, result.TransactionId);
        Assert.Equal("Success", result.TransactionStatus);
        Assert.Equal("00", result.ResponseCode);
        Assert.Equal(1999, result.AuthAmount);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.Equal("/api/payment", capturedRequest.RequestUri!.AbsolutePath);
        using var body = JsonDocument.Parse(await capturedRequest.Content!.ReadAsStringAsync());
        var header = body.RootElement.GetProperty("MessageHeader");
        Assert.True(Guid.TryParse(header.GetProperty("MessageID").GetString(), out _));
        Assert.Equal("App", header.GetProperty("Channel").GetString());
        var payment = body.RootElement.GetProperty("PaymentRequest");
        Assert.Equal(MerchantId, payment.GetProperty("MerchantID").GetGuid());
        Assert.Equal(TransactionId, payment.GetProperty("TransactionID").GetGuid());
        Assert.Equal(1999, payment.GetProperty("Amount").GetInt64());
        Assert.Equal("ZAR", payment.GetProperty("CurrencyCode").GetString());
        Assert.Equal("token-123", payment.GetProperty("Card").GetProperty("Token").GetString());
        Assert.Equal("CIT", payment.GetProperty("CardOnFileScenario").GetString());
        Assert.Equal(40, payment.GetProperty("CardAcceptorName").GetString()!.Length);
    }

    [Fact]
    public async Task QueryPaymentAsync_UsesMerchantAndTransactionIdAndReturnsNullForMissingPayment()
    {
        HttpRequestMessage? capturedRequest = null;
        using var httpClient = CreateClient(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });
        var client = CreateAcmeClient(httpClient);

        var result = await client.QueryPaymentAsync(TransactionId);

        Assert.Null(result);
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Get, capturedRequest.Method);
        Assert.Equal(
            $"/apirttq/query/Merchant/{MerchantId:D}?TransactionID={TransactionId:D}",
            capturedRequest.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task SubmitPaymentAsync_RetriesRateLimitedRequestsWithNewMessageIdsAndStableTransactionId()
    {
        var handler = new RateLimitThenSuccessHandler();
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://acme-test/")
        };
        var client = CreateAcmeClient(httpClient);

        var result = await client.SubmitPaymentAsync(CreateSubmission());

        Assert.Equal(TransactionId, result.TransactionId);
        Assert.Equal(3, handler.MessageIds.Count);
        Assert.Equal(3, handler.MessageIds.Distinct().Count());
        Assert.All(handler.TransactionIds, transactionId => Assert.Equal(TransactionId, transactionId));
    }

    [Fact]
    public async Task SubmitPaymentAsync_PropagatesGatewayStatusWhenErrorBodyIsNotJson()
    {
        using var httpClient = CreateClient(_ =>
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent("gateway unavailable")
            });
        var client = CreateAcmeClient(httpClient);

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
            client.SubmitPaymentAsync(CreateSubmission()));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
    }

    [Fact]
    public void CallbackAuthentication_ValidatesBasicAndRawBodyHmac()
    {
        const string username = "advice";
        const string password = "safe-local-test-password";
        var request = new DefaultHttpContext().Request;
        request.Headers.Authorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        Assert.True(AcmeCallbackAuthentication.IsValidBasicCredentials(request, username, password));
        Assert.False(AcmeCallbackAuthentication.IsValidBasicCredentials(request, username, "incorrect"));

        var rawBody = Encoding.UTF8.GetBytes("{\"transactionType\":\"Reversal\"}");
        const string secret = "safe-local-test-webhook-secret";
        var signature = Convert.ToBase64String(System.Security.Cryptography.HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), rawBody));
        Assert.True(AcmeCallbackAuthentication.IsValidSignature(rawBody, signature, secret));
        Assert.False(AcmeCallbackAuthentication.IsValidSignature(Encoding.UTF8.GetBytes("{}"), signature, secret));
    }

    [Fact]
    public void StubPaymentStore_AtomicallyReplaysTransactionAndRejectsRrnReuse()
    {
        var store = new StubPaymentStore();
        var payment = CreateRecord(TransactionId, "REFERENCE0001");
        var createdCount = 0;

        Parallel.For(0, 32, iteration =>
        {
            if (store.TryCreate(payment, out var existing, out var conflict) && existing is not null && conflict is null)
            {
                Interlocked.Increment(ref createdCount);
            }
        });

        Assert.Equal(1, createdCount);
        Assert.True(store.TryGet(TransactionId, out var existing));
        Assert.Equal(payment, existing);

        var conflicting = CreateRecord(Guid.NewGuid(), "REFERENCE0001");
        Assert.False(store.TryCreate(conflicting, out _, out var conflict));
        Assert.Equal(payment.TransactionId, conflict!.TransactionId);
    }

    [Fact]
    public async Task ApprovedNotificationHandler_PreservesCompletedEftStatus()
    {
        var paymentRepository = new Mock<IPaymentRepository>();
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            PaymentId = TransactionId.ToString("D"),
            AppPaymentId = "app-payment",
            Amount = 1999,
            Currency = "ZAR",
            SubMerchantRrn = "REFERENCE0001",
            Status = "Processing",
            AcmeTransactionUuid = TransactionId.ToString("D")
        };
        paymentRepository.Setup(repository => repository.GetByAppPaymentIdAsync("app-payment", It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);
        string? appliedStatus = null;
        PaymentChange? appliedChange = null;
        paymentRepository
            .Setup(repository => repository.ApplyNotificationAsync(
                It.IsAny<NotificationDelivery>(),
                It.IsAny<PaymentChange>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .Callback<NotificationDelivery, PaymentChange, string?, CancellationToken>((_, change, status, _) =>
            {
                appliedChange = change;
                appliedStatus = status;
            })
            .Returns(Task.CompletedTask);

        var body = new AcmeApprovedNotificationDto
        {
            EventType = "Approved",
            IsApproved = true,
            AcmeNotificationTransactionUuid = Guid.NewGuid(),
            Rrn = payment.SubMerchantRrn,
            Amount = payment.Amount,
            OccurredAt = DateTimeOffset.UtcNow,
            AppPaymentId = payment.AppPaymentId,
            RequestType = "Payment",
            MerchantID = MerchantId.ToString("D"),
            TransactionID = TransactionId.ToString("D"),
            TransactionDateTime = DateTime.UtcNow,
            OrderNumber = "REFERENCE0001",
            ReconID = "recon-id",
            RetrievalReferenceNumber = payment.SubMerchantRrn,
            SystemsTraceAuditNumber = "123456",
            CurrencyCode = "ZAR",
            PaymentService = "CardNotPresent",
            TransactionStatus = "Complete",
            ResponseCode = "COMPL",
            ResponseDescription = "Complete",
            AuthCode = "ABC123",
            MaskedCardNumber = "541333******1429",
            CommProtocol = "REST"
        };
        var handler = new ProcessAcmeApprovedNotificationCommandHandler(paymentRepository.Object);
        var command = new ProcessAcmeApprovedNotificationCommand(
            "Payment",
            true,
            Guid.NewGuid(),
            payment.SubMerchantRrn,
            payment.Amount,
            DateTimeOffset.UtcNow,
            payment.AppPaymentId,
            body);

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal("Complete", appliedStatus);
        Assert.Equal("Approved", appliedChange!.EventType);
    }

    private static HttpClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> responseFactory) =>
        new(new StubHttpMessageHandler(responseFactory))
        {
            BaseAddress = new Uri("http://acme-test/")
        };

    private static AcmePayClient CreateAcmeClient(HttpClient httpClient)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AcmePay:MerchantId"] = MerchantId.ToString("D")
            })
            .Build();
        return new AcmePayClient(httpClient, configuration);
    }

    private static HttpResponseMessage JsonResponse(string json) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

    private static AcmePaymentSubmission CreateSubmission() =>
        new(
            TransactionId,
            "REFERENCE0001",
            1999,
            "ZAR",
            "CardNotPresent",
            "token-123",
            "CIT",
            "ZAP0004182",
            "5812",
            "ZAP*CAFE DEL SOL CAPE TOWN WCZA         ",
            "alice@example.com");

    private static StubPaymentRecord CreateRecord(Guid transactionId, string rrn) =>
        new(transactionId, rrn, rrn, 1999, "ZAR", "CardNotPresent", "ZAP0004182", "5812",
            "ZAP*CAFE DEL SOL CAPE TOWN WCZA         ", "alice@example.com", "Processing", "PEND",
            "Payment is being processed.", string.Empty, DateTimeOffset.UtcNow);

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(responseFactory(request));
    }

    private sealed class RateLimitThenSuccessHandler : HttpMessageHandler
    {
        public List<Guid> MessageIds { get; } = [];
        public List<Guid> TransactionIds { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(cancellationToken));
            MessageIds.Add(body.RootElement.GetProperty("MessageHeader").GetProperty("MessageID").GetGuid());
            TransactionIds.Add(body.RootElement.GetProperty("PaymentRequest").GetProperty("TransactionID").GetGuid());
            if (MessageIds.Count <= 2)
            {
                var limited = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
                limited.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromMilliseconds(5));
                return limited;
            }

            return JsonResponse($$"""
                {
                  "PaymentResponse": {
                    "TransactionID": "{{TransactionId}}",
                    "TransactionStatus": "Success",
                    "ResponseDetail": { "Code": "00" }
                  }
                }
                """);
        }
    }
}
