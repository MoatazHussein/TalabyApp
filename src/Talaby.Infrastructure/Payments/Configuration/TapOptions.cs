using System.ComponentModel.DataAnnotations;

namespace Talaby.Infrastructure.Payments.Configuration;

public sealed class TapOptions
{
    public const string SectionName = "Tap";

    [Required]
    public string BaseUrl { get; init; } = "https://api.tap.company";

    public string? SecretKey { get; init; }

    public string? MerchantId { get; init; }
}