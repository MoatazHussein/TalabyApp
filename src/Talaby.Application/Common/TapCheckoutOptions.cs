namespace Talaby.Application.Common;

public sealed class TapCheckoutOptions
{
    public const string SectionName = "Tap";

    public string SourceId { get; init; } = default!;

    public string FrontendBaseUrl { get; init; } = default!;

    public string ApiPublicBaseUrl { get; init; } = default!;

    public decimal CommissionPercentage { get; init; }
}
