using Talaby.Domain.Constants;

namespace Talaby.Application.Users.Commands.Login
{
    public class LoginResponseDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public int UserTypeValue { get; set; } 
        public string UserTypeName { get; set; } 
        public ICollection<string> Roles { get; set; } 
    }

}
