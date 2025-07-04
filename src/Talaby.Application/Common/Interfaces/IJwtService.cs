using Talaby.Domain.Entities;

namespace Talaby.Application.Common.Interfaces;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(AppUser user);
}
