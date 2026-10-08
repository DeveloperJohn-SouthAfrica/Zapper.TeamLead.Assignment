using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AcmeIntegreatedAssessement.Api.Components.Pages;

public partial class Home
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    private DailySummaryModel? _summary;
    private bool _isLoading = true;
    private string? _errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var envelope = await Http.GetFromJsonAsync<MessageEnvelopeDto<DailySummaryDto>>("api/reports/daily-summary");
            if (envelope?.Payload is { } dto)
            {
                _summary = DailySummaryModel.FromDto(dto);
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Unable to reach the summary service: {ex.Message}";
        }
        finally
        {
            _isLoading = false;
        }
    }

    private string ReportDateDisplay =>
        _summary?.ReportDate?.ToString("dddd, dd MMM yyyy") ??
        DateOnly.FromDateTime(DateTime.UtcNow.AddHours(2)).ToString("dddd, dd MMM yyyy");

    private SummaryTotals Totals => _summary?.SummaryTotals ?? SummaryTotals.Empty;

    private string TotalsDisplay =>
        _summary?.SubMerchantSummaries.Select(row => row.Currency).Distinct(StringComparer.OrdinalIgnoreCase).ToArray() is { Length: 1 } currencies
            ? FormatCurrency(Totals.TotalAmount, currencies[0])
            : "Multiple currencies";

    private IReadOnlyList<HeroMetric> HeroMetrics =>
    [
        new("Approved events", Totals.ApprovedCount.ToString("N0"), FormatAmount(Totals.ApprovedAmount), "Acme response", "stat-approved", Color.Success),
        new("Declined events", Totals.DeclinedCount.ToString("N0"), FormatAmount(Totals.DeclinedAmount), "Acme response", "stat-declined", Color.Error),
        new("Reversals", Totals.ReversedCount.ToString("N0"), FormatAmount(Totals.ReversedAmount), "Switch outcome", "stat-reversed", Color.Warning),
        new("Refunds", Totals.RefundedCount.ToString("N0"), FormatAmount(Totals.RefundedAmount), "Switch outcome", "stat-refunded", Color.Info)
    ];

    private IReadOnlyList<SubMerchantRow> MerchantRows => _summary?.SubMerchantSummaries ?? Array.Empty<SubMerchantRow>();

    private static readonly InfrastructureItem[] InfrastructureItems =
    {
        new("SQL Server", "Payments, lifecycle changes, callback deliveries, and settlement summaries are persisted in SQL Server.", "Configured", "Schema packaged as a DACPAC", Color.Primary, "infra-sql"),
        new("Redis", "Redis is available in the local compose stack for the existing cache and messaging integrations.", "Configured", "Not used as payment system-of-record", Color.Success, "infra-redis"),
        new("AWS Secrets Manager", "The API can load secrets from AWS Secrets Manager when enabled; local development uses environment configuration.", "Optional", "No AWS AppConfig integration", Color.Tertiary, "infra-secrets")
    };

    private string FormatAmount(long amount) =>
        _summary?.SubMerchantSummaries.Select(row => row.Currency).Distinct(StringComparer.OrdinalIgnoreCase).ToArray() is { Length: 1 } currencies
            ? FormatCurrency(amount, currencies[0])
            : "By currency";

    private static string FormatCurrency(long amount, string currency) =>
        $"{currency} {(amount / 100m).ToString("N2", CultureInfo.GetCultureInfo("en-ZA"))}";

    private sealed class DailySummaryModel
    {
        public DateOnly? ReportDate { get; init; }
        public IReadOnlyList<SubMerchantRow> SubMerchantSummaries { get; init; } = Array.Empty<SubMerchantRow>();

        public SummaryTotals SummaryTotals => new(
            ApprovedCount: SubMerchantSummaries.Sum(m => m.ApprovedCount),
            ApprovedAmount: SubMerchantSummaries.Sum(m => m.ApprovedAmount),
            DeclinedCount: SubMerchantSummaries.Sum(m => m.DeclinedCount),
            DeclinedAmount: SubMerchantSummaries.Sum(m => m.DeclinedAmount),
            ReversedCount: SubMerchantSummaries.Sum(m => m.ReversedCount),
            ReversedAmount: SubMerchantSummaries.Sum(m => m.ReversedAmount),
            RefundedCount: SubMerchantSummaries.Sum(m => m.RefundedCount),
            RefundedAmount: SubMerchantSummaries.Sum(m => m.RefundedAmount));

        public static DailySummaryModel FromDto(DailySummaryDto dto)
        {
            var merchants = dto.SubMerchantSummaries
                .Where(item => (item.ApprovedCount + item.DeclinedCount + item.ReversedCount + item.RefundedCount) > 0)
                .Select(item => new SubMerchantRow(
                    item.SubMerchantId,
                    item.Currency,
                    item.ApprovedCount,
                    item.ApprovedAmount,
                    item.DeclinedCount,
                    item.DeclinedAmount,
                    item.ReversedCount,
                    item.ReversedAmount,
                    item.RefundedCount,
                    item.RefundedAmount))
                .OrderByDescending(item => item.TotalAmount)
                .ThenBy(item => item.SubMerchantId)
                .ToList();

            return new DailySummaryModel
            {
                ReportDate = DateOnly.TryParse(dto.ReportDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed) ? parsed : null,
                SubMerchantSummaries = merchants
            };
        }
    }

    private sealed record SubMerchantRow(
        Guid SubMerchantId,
        string Currency,
        long ApprovedCount,
        long ApprovedAmount,
        long DeclinedCount,
        long DeclinedAmount,
        long ReversedCount,
        long ReversedAmount,
        long RefundedCount,
        long RefundedAmount)
    {
        public long TotalEvents => ApprovedCount + DeclinedCount + ReversedCount + RefundedCount;
        public long TotalAmount => ApprovedAmount + DeclinedAmount + ReversedAmount + RefundedAmount;
        public string TotalAmountDisplay => TotalAmount == 0 ? "-" : FormatCurrency(TotalAmount, Currency);
    }

    private readonly record struct SummaryTotals(
        long ApprovedCount,
        long ApprovedAmount,
        long DeclinedCount,
        long DeclinedAmount,
        long ReversedCount,
        long ReversedAmount,
        long RefundedCount,
        long RefundedAmount)
    {
        public static SummaryTotals Empty => new(0, 0, 0, 0, 0, 0, 0, 0);
        public long EventCount => ApprovedCount + DeclinedCount + ReversedCount + RefundedCount;
        public long TotalAmount => ApprovedAmount + DeclinedAmount + ReversedAmount + RefundedAmount;
    }

    private sealed record HeroMetric(string Label, string CountDisplay, string AmountDisplay, string TagLine, string TestId, Color ChipColor);

    private sealed record InfrastructureItem(string Title, string Description, string Status, string Footnote, Color ChipColor, string TestId);

    private sealed class DailySummaryDto
    {
        [JsonPropertyName("reportDate")]
        public string ReportDate { get; init; } = string.Empty;

        [JsonPropertyName("subMerchantSummaries")]
        public IReadOnlyList<SubMerchantDto> SubMerchantSummaries { get; init; } = Array.Empty<SubMerchantDto>();
    }

    private sealed class MessageEnvelopeDto<T>
    {
        [JsonPropertyName("payload")]
        public T? Payload { get; init; }
    }

    private sealed class SubMerchantDto
    {
        [JsonPropertyName("subMerchantId")]
        public Guid SubMerchantId { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; } = string.Empty;

        [JsonPropertyName("approvedCount")]
        public long ApprovedCount { get; init; }

        [JsonPropertyName("approvedAmount")]
        public long ApprovedAmount { get; init; }

        [JsonPropertyName("declinedCount")]
        public long DeclinedCount { get; init; }

        [JsonPropertyName("declinedAmount")]
        public long DeclinedAmount { get; init; }

        [JsonPropertyName("reversedCount")]
        public long ReversedCount { get; init; }

        [JsonPropertyName("reversedAmount")]
        public long ReversedAmount { get; init; }

        [JsonPropertyName("refundedCount")]
        public long RefundedCount { get; init; }

        [JsonPropertyName("refundedAmount")]
        public long RefundedAmount { get; init; }
    }
}
