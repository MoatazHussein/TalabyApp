using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Users
{
    public interface IUserContext
    {
        CurrentUser? GetCurrentUser();
    }

    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public CurrentUser? GetCurrentUser()
        {
            var user = httpContextAccessor?.HttpContext?.User;
            if (user == null)
            {
                throw new InvalidOperationException("AppUser context is not present");
            }

            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return null;
            }

            var userId = Guid.TryParse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value, out var id) ? id : throw new UnAuthorizedAccessException("User ID invalid");
            var email = user.FindFirst(c => c.Type == ClaimTypes.Email)!.Value;
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role)!.Select(c => c.Value);

            return new CurrentUser(userId, email, roles);
        }
    }
}