namespace Talaby.Application.Features.Users.Services;

public record CurrentUser(Guid Id,
    string Email,
    IEnumerable<string> Roles)
{
    public bool IsInRole(string role) => Roles.Contains(role);
}
