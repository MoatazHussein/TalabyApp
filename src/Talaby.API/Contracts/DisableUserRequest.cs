namespace Talaby.API.Contracts;

public sealed record DisableUserRequest(DateTimeOffset? DisabledUntil);
