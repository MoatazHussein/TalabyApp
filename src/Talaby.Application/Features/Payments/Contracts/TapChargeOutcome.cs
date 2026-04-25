namespace Talaby.Application.Features.Payments.Contracts;

public enum TapChargeOutcome
{
    Success,

    TerminalFailure,

    NonFinal
}

public static class TapChargeStatusMapper
{
    private const string CapturedStatus = "CAPTURED";

    private static readonly HashSet<string> TerminalFailureStatuses =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "DECLINED", "FAILED", "CANCELLED", "ABANDONED",
            "RESTRICTED", "TIMEDOUT", "VOID"
        };

    public static TapChargeOutcome Map(string? providerStatus)
    {
        if (string.IsNullOrWhiteSpace(providerStatus))
            return TapChargeOutcome.NonFinal;

        if (string.Equals(providerStatus, CapturedStatus, StringComparison.OrdinalIgnoreCase))
            return TapChargeOutcome.Success;

        if (TerminalFailureStatuses.Contains(providerStatus))
            return TapChargeOutcome.TerminalFailure;

        return TapChargeOutcome.NonFinal;
    }
}
