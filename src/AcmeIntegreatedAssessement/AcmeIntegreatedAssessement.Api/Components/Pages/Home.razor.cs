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
            var dto = await Http.GetFromJsonAsync<DailySummaryDto>("api/reports/daily-summary");
            if (dto is not null)
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
        DateOnly.FromDateTime(DateTime.UtcNow).ToString("dddd, dd MMM yyyy");

    private SummaryTotals Totals => _summary?.SummaryTotals ?? SummaryTotals.Empty;

    private string TotalsDisplay => FormatCurrency(Totals.TotalAmount);

    private IReadOnlyList<HeroMetric> HeroMetrics => HeroMetric.Build(Totals);

    private IReadOnlyList<SubMerchantRow> MerchantRows => _summary?.SubMerchantSummaries ?? Array.Empty<SubMerchantRow>();

    private static readonly InfrastructureItem[] InfrastructureItems =
    {
        new("SQL Server (DACPAC)", "Schema deployed via SQLPackage, every stored procedure owning its own model so changes are isolated and parameter sniffing is stabilized.", "Live", "DACPAC + stored-procedure parameter guards", Color.Primary, "infra-sql"),
        new("Redis cache", "Free-tier Redis stays stateful, caching each token/card individually and flushing per write so the stateless .NET layer scales on demand.", "Healthy", "Straight-through flush on writes", Color.Success, "infra-redis"),
        new("AWS AppConfig + Feature Flags", "AppConfig drives feature toggles (env var overrides take precedence) so the UI and tests read the same config fingerprint.", "Synced", "Feature flags backed by AppConfig & env vars", Color.Tertiary, "infra-appconfig")
    };

    private static string FormatCurrency(decimal value) =>
        value.ToString("C0", CultureInfo.CurrentCulture);

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
                    item.SubMerchantRrn,
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
                .ThenBy(item => item.SubMerchantRrn, StringComparer.Ordinal)
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
        string SubMerchantRrn,
        string Currency,
        long ApprovedCount,
        decimal ApprovedAmount,
        long DeclinedCount,
        decimal DeclinedAmount,
        long ReversedCount,
        decimal ReversedAmount,
        long RefundedCount,
        decimal RefundedAmount)
    {
        public long TotalEvents => ApprovedCount + DeclinedCount + ReversedCount + RefundedCount;
        public decimal TotalAmount => ApprovedAmount + DeclinedAmount + ReversedAmount + RefundedAmount;
        public string TotalAmountDisplay => TotalAmount == 0 ? "-" : FormatCurrency(TotalAmount);
    }

    private readonly record struct SummaryTotals(
        long ApprovedCount,
        decimal ApprovedAmount,
        long DeclinedCount,
        decimal DeclinedAmount,
        long ReversedCount,
        decimal ReversedAmount,
        long RefundedCount,
        decimal RefundedAmount)
    {
        public static SummaryTotals Empty => new(0, 0m, 0, 0m, 0, 0m, 0, 0m);
        public long EventCount => ApprovedCount + DeclinedCount + ReversedCount + RefundedCount;
        public decimal TotalAmount => ApprovedAmount + DeclinedAmount + ReversedAmount + RefundedAmount;
    }

    private sealed record HeroMetric(string Label, string CountDisplay, string AmountDisplay, string TagLine, string TestId, Color ChipColor)
    {
        public static IReadOnlyList<HeroMetric> Build(SummaryTotals totals) => new[]
        {
            new HeroMetric("Approved events", totals.ApprovedCount.ToString("N0"), FormatCurrency(totals.ApprovedAmount), "Consent secured", "stat-approved", Color.Success),
            new HeroMetric("Declined events", totals.DeclinedCount.ToString("N0"), FormatCurrency(totals.DeclinedAmount), "Acquirer feedback", "stat-declined", Color.Error),
            new HeroMetric("Reversals", totals.ReversedCount.ToString("N0"), FormatCurrency(totals.ReversedAmount), "Reconciliation", "stat-reversed", Color.Warning),
            new HeroMetric("Refunds", totals.RefundedCount.ToString("N0"), FormatCurrency(totals.RefundedAmount), "Customer care", "stat-refunded", Color.Info)
        };
    }

    private sealed record InfrastructureItem(string Title, string Description, string Status, string Footnote, Color ChipColor, string TestId);

    private sealed class DailySummaryDto
    {
        [JsonPropertyName("reportDate")]
        public string ReportDate { get; init; } = string.Empty;

        [JsonPropertyName("subMerchantSummaries")]
        public IReadOnlyList<SubMerchantDto> SubMerchantSummaries { get; init; } = Array.Empty<SubMerchantDto>();
    }

    private sealed class SubMerchantDto
    {
        [JsonPropertyName("subMerchantId")]
        public Guid SubMerchantId { get; init; }

        [JsonPropertyName("subMerchantRrn")]
        public string SubMerchantRrn { get; init; } = string.Empty;

        [JsonPropertyName("currency")]
        public string Currency { get; init; } = string.Empty;

        [JsonPropertyName("approvedCount")]
        public long ApprovedCount { get; init; }

        [JsonPropertyName("approvedAmount")]
        public decimal ApprovedAmount { get; init; }

        [JsonPropertyName("declinedCount")]
        public long DeclinedCount { get; init; }

        [JsonPropertyName("declinedAmount")]
        public decimal DeclinedAmount { get; init; }

        [JsonPropertyName("reversedCount")]
        public long ReversedCount { get; init; }

        [JsonPropertyName("reversedAmount")]
        public decimal ReversedAmount { get; init; }

        [JsonPropertyName("refundedCount")]
        public long RefundedCount { get; init; }

        [JsonPropertyName("refundedAmount")]
        public decimal RefundedAmount { get; init; }
    }
}
