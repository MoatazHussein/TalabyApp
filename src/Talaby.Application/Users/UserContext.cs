using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Users
{
    public interface IUserContext
    {
        CurrentUser GetCurrentUser();
    }

    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CurrentUser GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
                throw new UnAuthorizedAccessException("User is not authenticated.");

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnAuthorizedAccessException("Invalid or missing User ID.");

            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? throw new UnAuthorizedAccessException("Email claim is missing.");

            var roles = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            return new CurrentUser(userId, email, roles);
        }
    }

}