namespace Talaby.Application.Features.Payments.Commands.CreateProjectCommissionCheckout;

public sealed record CreateProjectCommissionCheckoutResponse(
    Guid ProjectCommissionPaymentId,
    Guid ProjectCommissionPaymentAttemptId,
    string ProviderChargeId,
    string CheckoutUrl,
    string ProviderStatus);
