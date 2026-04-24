namespace Talaby.Domain.Exceptions;

public sealed class PaymentGatewayException(string message) : Exception(message);
