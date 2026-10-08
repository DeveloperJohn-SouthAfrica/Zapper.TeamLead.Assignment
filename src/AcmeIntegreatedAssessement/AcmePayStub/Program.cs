using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddSingleton<StubPaymentStore>();
builder.Services.AddHttpClient();

var app = builder.Build();
var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = null
};
var accountMerchantId = Guid.TryParse(
    Environment.GetEnvironmentVariable("ACME_PAY_MERCHANT_ID"),
    out var configuredMerchantId)
    ? configuredMerchantId
    : Guid.Parse("8B67BF88-BB8D-4EB5-ADFC-0F5C97C7ED67");
var apiKey = Environment.GetEnvironmentVariable("ACME_STUB_API_KEY");

app.MapGet("/hc", () => Results.Ok(new { Status = "Healthy" }));

app.MapPost("/api/payment", async (
    HttpRequest request,
    StubPaymentStore store,
    IHttpClientFactory clients,
    ILoggerFactory loggerFactory) =>
{
    if (!HasValidBearer(request, apiKey))
    {
        return Results.Unauthorized();
    }

    JsonDocument document;
    try
    {
        document = await JsonDocument.ParseAsync(request.Body, cancellationToken: request.HttpContext.RequestAborted);
    }
    catch (JsonException)
    {
        return Results.BadRequest();
    }

    using (document)
    {
        if (!document.RootElement.TryGetProperty("MessageHeader", out var header) ||
            !document.RootElement.TryGetProperty("PaymentRequest", out var paymentRequest) ||
            !IsValidHeader(header) ||
            !Guid.TryParse(ReadString(paymentRequest, "MerchantID"), out var merchantId) ||
            merchantId != accountMerchantId ||
            !Guid.TryParse(ReadString(paymentRequest, "TransactionID"), out var transactionId) ||
            !TryReadPayment(paymentRequest, transactionId, out var payment))
        {
            return Results.Json(CreateErrorEnvelope(Guid.Empty, "E01", "Invalid payment request."),
                jsonOptions,
                statusCode: StatusCodes.Status400BadRequest);
        }

        var scenario = (Environment.GetEnvironmentVariable("ACME_STUB_SCENARIO") ?? "approved").Trim().ToLowerInvariant();
        var record = CreatePaymentResult(payment, scenario);
        if (!store.TryCreate(record, out var savedPayment, out var conflict))
        {
            if (conflict is not null)
            {
                return Results.Json(
                    CreateErrorEnvelope(conflict.TransactionId, "E09", "Retrieval Reference Number already in use."),
                    jsonOptions,
                    statusCode: StatusCodes.Status409Conflict);
            }
            return Results.Json(CreatePaymentEnvelope(savedPayment!), jsonOptions);
        }

        record = savedPayment!;

        if (scenario == "gateway-error")
        {
            return Results.Json(
                CreateErrorEnvelope(record.TransactionId, "E50", "The gateway is degraded."),
                jsonOptions,
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        if (scenario == "timeout")
        {
            await Task.Delay(TimeSpan.FromSeconds(35), CancellationToken.None);
        }

        if (Environment.GetEnvironmentVariable("ACME_STUB_CALLBACKS_ENABLED") == "true")
        {
            await DeliverScenarioCallbacksAsync(
                scenario,
                record,
                clients.CreateClient(),
                loggerFactory.CreateLogger("AcmePayStub.Callbacks"));
        }

        return Results.Json(CreatePaymentEnvelope(record), jsonOptions);
    }
});

app.MapGet("/apirttq/query/Merchant/{merchantId}", (
    HttpRequest request,
    string merchantId,
    StubPaymentStore store) =>
{
    if (!HasValidBearer(request, apiKey))
    {
        return Results.Unauthorized();
    }

    if (!Guid.TryParse(merchantId, out var requestedMerchantId) || requestedMerchantId != accountMerchantId)
    {
        return Results.NotFound();
    }

    StubPaymentRecord? payment = null;
    if (Guid.TryParse(request.Query["TransactionID"], out var transactionId))
    {
        store.TryGet(transactionId, out payment);
    }
    else if (request.Query.TryGetValue("OrderNumber", out var orderNumber))
    {
        store.TryGetByRetrievalReference(orderNumber.ToString()[..Math.Min(12, orderNumber.ToString().Length)], out payment);
    }

    return payment is null
        ? Results.NotFound()
        : Results.Json(CreatePaymentEnvelope(payment), jsonOptions);
});

app.Run();

static bool HasValidBearer(HttpRequest request, string? expectedApiKey)
{
    var value = request.Headers.Authorization.ToString();
    return !string.IsNullOrWhiteSpace(expectedApiKey) &&
           value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) &&
           CryptographicOperations.FixedTimeEquals(
               Encoding.UTF8.GetBytes(value[7..].Trim()),
               Encoding.UTF8.GetBytes(expectedApiKey));
}

static bool IsValidHeader(JsonElement header) =>
    header.ValueKind == JsonValueKind.Object &&
    Guid.TryParse(ReadString(header, "MessageID"), out var messageId) &&
    messageId != Guid.Empty &&
    ReadString(header, "Channel") is "App" or "Web" or "POS" &&
    DateTimeOffset.TryParse(ReadString(header, "MessageDateTime"), CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

static string? ReadString(JsonElement element, string propertyName) =>
    element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
        ? value.GetString()
        : null;

static bool TryReadPayment(JsonElement request, Guid transactionId, out StubPaymentRecord payment)
{
    payment = null!;
    var orderNumber = ReadString(request, "OrderNumber");
    var currencyCode = ReadString(request, "CurrencyCode") ?? "ZAR";
    var paymentService = ReadString(request, "PaymentService");
    var subMerchantId = ReadString(request, "SubMerchantID");
    var mcc = ReadString(request, "MerchantCategoryCode");
    var acceptorName = ReadString(request, "CardAcceptorName");
    var email = ReadString(request, "Email") ?? string.Empty;
    var cardOnFileScenario = ReadString(request, "CardOnFileScenario");
    if (string.IsNullOrWhiteSpace(orderNumber) ||
        !orderNumber.All(char.IsLetterOrDigit) ||
        orderNumber.Length > 50 ||
        !request.TryGetProperty("Amount", out var amountElement) ||
        !amountElement.TryGetInt64(out var amount) ||
        amount <= 0 ||
        currencyCode.Length != 3 ||
        paymentService is not ("CardNotPresent" or "CardNotPresentRecurring" or "AcmeEft" or "GooglePay" or "ApplePay") ||
        string.IsNullOrWhiteSpace(subMerchantId) ||
        subMerchantId.Length > 15 ||
        string.IsNullOrWhiteSpace(mcc) ||
        mcc.Length > 4 ||
        string.IsNullOrWhiteSpace(acceptorName) ||
        acceptorName.Length != 40 ||
        !request.TryGetProperty("Card", out var card) ||
        string.IsNullOrWhiteSpace(ReadString(card, "Token")) ||
        cardOnFileScenario is not ("CIT" or "MIT"))
    {
        return false;
    }

    payment = new StubPaymentRecord(
        transactionId,
        orderNumber,
        orderNumber[..12],
        amount,
        currencyCode,
        paymentService,
        subMerchantId,
        mcc,
        acceptorName,
        email,
        "Processing",
        "PEND",
        "Processing",
        string.Empty,
        DateTimeOffset.UtcNow);
    return true;
}

static StubPaymentRecord CreatePaymentResult(StubPaymentRecord request, string scenario)
{
    var (status, code, description) = scenario switch
    {
        "declined" => ("Success", "05", "Do not honour"),
        "processing" or "timeout" => ("Processing", "PEND", "Payment is being processed."),
        "complete" => ("Complete", "COMPL", "EFT collection completed."),
        "gateway-error" => ("Failure", "E50", "The gateway is degraded."),
        _ => ("Success", "00", "Approved or completed successfully.")
    };

    return request with
    {
        TransactionStatus = status,
        ResponseCode = code,
        ResponseDescription = description,
        AuthCode = code == "00" ? "238699" : string.Empty
    };
}

static object CreatePaymentEnvelope(StubPaymentRecord payment) =>
    new
    {
        MessageHeader = CreateHeader(),
        PaymentResponse = new
        {
            TransactionID = payment.TransactionId,
            TransactionDateTime = payment.CreatedAt.ToOffset(TimeSpan.FromHours(2)).ToString("yyyy-MM-dd'T'HH:mm:ss.fff"),
            OrderNumber = payment.OrderNumber,
            ReconID = "MR747891457",
            RetrievalReferenceNumber = payment.RetrievalReferenceNumber,
            AuthCode = payment.AuthCode,
            TransactionStatus = payment.TransactionStatus,
            ResponseDetail = new
            {
                Source = "Acquirer",
                Code = payment.ResponseCode,
                Description = payment.ResponseDescription,
                ClientMessage = payment.ResponseCode == "00" ? "Approved." : payment.ResponseDescription
            },
            AuthAmount = payment.ResponseCode == "00" ? payment.Amount : 0,
            FutureResponseField = "ignored by clients"
        }
    };

static object CreateErrorEnvelope(Guid transactionId, string code, string description) =>
    new
    {
        MessageHeader = CreateHeader(),
        PaymentResponse = new
        {
            TransactionID = transactionId,
            TransactionStatus = "Failure",
            ResponseDetail = new { Source = "Application", Code = code, Description = description }
        }
    };

static object CreateHeader() => new
{
    MessageDateTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(2)).ToString("yyyy-MM-dd'T'HH:mm:ss.fff"),
    MessageID = Guid.NewGuid(),
    Channel = "App"
};

static async Task DeliverScenarioCallbacksAsync(
    string scenario,
    StubPaymentRecord payment,
    HttpClient client,
    ILogger logger)
{
    var adviceUrl = Environment.GetEnvironmentVariable("ACME_ADVICE_CALLBACK_URL");
    if (string.IsNullOrWhiteSpace(adviceUrl))
    {
        logger.LogWarning("Callback delivery is enabled but ACME_ADVICE_CALLBACK_URL is not configured.");
        return;
    }

    var adviceTime = payment.CreatedAt.ToOffset(TimeSpan.FromHours(2)).ToString("yyyy-MM-dd'T'HH:mm:ss.fff");
    var approved = payment.ResponseCode == "00";
    var advice = new
    {
        RequestType = "Payment",
        MerchantID = Environment.GetEnvironmentVariable("ACME_PAY_MERCHANT_ID") ?? "8B67BF88-BB8D-4EB5-ADFC-0F5C97C7ED67",
        TransactionID = payment.TransactionId,
        TransactionDateTime = adviceTime,
        OrderNumber = payment.OrderNumber,
        ReconID = "MR747891457",
        RetrievalReferenceNumber = payment.RetrievalReferenceNumber,
        SystemsTraceAuditNumber = "7543",
        Amount = payment.Amount,
        CurrencyCode = payment.CurrencyCode,
        PaymentService = payment.PaymentService,
        TransactionStatus = payment.TransactionStatus,
        ResponseSource = "Acquirer",
        ResponseCode = payment.ResponseCode,
        ResponseDescription = payment.ResponseDescription,
        AuthCode = payment.AuthCode,
        MaskedCardNumber = "541333******1429",
        CommProtocol = "REST"
    };

    if (scenario == "out-of-order")
    {
        await SendAdjustmentAsync(client, payment, "Reversal", payment.CreatedAt.AddMinutes(1), true, logger);
        await SendAdviceAsync(client, adviceUrl, advice, logger);
        return;
    }

    await SendAdviceAsync(client, adviceUrl, advice, logger);
    if (scenario is "duplicate")
    {
        await SendAdviceAsync(client, adviceUrl, advice, logger);
    }
    else if (scenario is "reversal" or "refund")
    {
        await SendAdjustmentAsync(
            client,
            payment,
            scenario == "refund" ? "Refund" : "Reversal",
            payment.CreatedAt.AddMinutes(1),
            approved,
            logger);
    }
}

static async Task SendAdviceAsync(HttpClient client, string url, object payload, ILogger logger)
{
    var username = Environment.GetEnvironmentVariable("ACME_ADVICE_USERNAME");
    var password = Environment.GetEnvironmentVariable("ACME_ADVICE_PASSWORD");
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        logger.LogWarning("Advice callback delivery skipped because credentials are not configured.");
        return;
    }

    await SendWithRetriesAsync(client, () =>
    {
        var message = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(payload, options: new JsonSerializerOptions { PropertyNamingPolicy = null })
        };
        message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
        return message;
    }, logger);
}

static async Task SendAdjustmentAsync(
    HttpClient client,
    StubPaymentRecord payment,
    string type,
    DateTimeOffset occurredAt,
    bool isApproved,
    ILogger logger)
{
    var url = Environment.GetEnvironmentVariable("ACME_ADJUSTMENT_CALLBACK_URL");
    var secret = Environment.GetEnvironmentVariable("ACME_WEBHOOK_SECRET");
    if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(secret))
    {
        logger.LogWarning("Adjustment callback delivery skipped because its URL or signing secret is not configured.");
        return;
    }

    var body = JsonSerializer.Serialize(new
    {
        transactionUUID = Guid.NewGuid(),
        transactionType = type,
        transactionDateTime = occurredAt.ToOffset(TimeSpan.FromHours(2)).ToString("yyyy-MM-dd'T'HH:mm:ss.fff"),
        rrn = payment.RetrievalReferenceNumber,
        transactionAmount = payment.Amount,
        currencyCode = payment.CurrencyCode,
        txResultCode = isApproved ? "00" : "05",
        isApproved
    }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    var signature = Convert.ToBase64String(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(body)));
    await SendWithRetriesAsync(client, () =>
    {
        var message = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
        message.Headers.Add("x-signature", signature);
        return message;
    }, logger);
}

static async Task SendWithRetriesAsync(HttpClient client, Func<HttpRequestMessage> createMessage, ILogger logger)
{
    for (var attempt = 1; attempt <= 4; attempt++)
    {
        using var message = createMessage();
        using var response = await client.SendAsync(message, CancellationToken.None);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return;
        }

        logger.LogWarning("Callback delivery attempt {Attempt} received HTTP {StatusCode}.", attempt, (int)response.StatusCode);
        if (attempt < 4)
        {
            await Task.Delay(TimeSpan.FromSeconds(attempt));
        }
    }
}

public sealed record StubPaymentRecord(
    Guid TransactionId,
    string OrderNumber,
    string RetrievalReferenceNumber,
    long Amount,
    string CurrencyCode,
    string PaymentService,
    string SubMerchantId,
    string MerchantCategoryCode,
    string CardAcceptorName,
    string Email,
    string TransactionStatus,
    string ResponseCode,
    string ResponseDescription,
    string AuthCode,
    DateTimeOffset CreatedAt);

public sealed class StubPaymentStore
{
    private readonly object _sync = new();
    private readonly Dictionary<Guid, StubPaymentRecord> _byTransactionId = [];
    private readonly Dictionary<string, Guid> _byRetrievalReference = new(StringComparer.Ordinal);

    public bool TryCreate(StubPaymentRecord payment, out StubPaymentRecord? existingPayment, out StubPaymentRecord? conflict)
    {
        lock (_sync)
        {
            if (_byTransactionId.TryGetValue(payment.TransactionId, out existingPayment))
            {
                conflict = null;
                return false;
            }

            if (_byRetrievalReference.TryGetValue(payment.RetrievalReferenceNumber, out var conflictingTransactionId))
            {
                conflict = _byTransactionId[conflictingTransactionId];
                return false;
            }

            _byRetrievalReference.Add(payment.RetrievalReferenceNumber, payment.TransactionId);
            _byTransactionId.Add(payment.TransactionId, payment);
            existingPayment = payment;
            conflict = null;
            return true;
        }
    }

    public bool TryGetByRetrievalReference(string rrn, out StubPaymentRecord? payment)
    {
        lock (_sync)
        {
            payment = null;
            return _byRetrievalReference.TryGetValue(rrn, out var transactionId) &&
                   _byTransactionId.TryGetValue(transactionId, out payment);
        }
    }

    public bool TryGet(Guid transactionId, out StubPaymentRecord? payment)
    {
        lock (_sync)
        {
            return _byTransactionId.TryGetValue(transactionId, out payment);
        }
    }
}
