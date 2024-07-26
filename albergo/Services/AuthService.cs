using albergo.Models;
using BCrypt.Net;
using Albergo.Repositories;

namespace Albergo.Services
{
    public interface IAuthService
    {
        LoginModel Login(string username, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public LoginModel Login(string username, string password)
        {
            var user = _userRepository.GetByUsername(username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }

            // Optionally log failed login attempts or handle security concerns here
            return null;
        }
    }
}
