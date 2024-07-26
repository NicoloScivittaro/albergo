using albergo.Models;
using System.Collections.Generic;
using System.Linq;
using BCrypt.Net;

namespace Albergo.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<LoginModel> _users;

        public UserRepository()
        {
            _users = new List<LoginModel>
            {
                new LoginModel { Id = 1, Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("adminpass"), Role = "admin" },
                new LoginModel { Id = 2, Username = "receptionist", PasswordHash = BCrypt.Net.BCrypt.HashPassword("receptionpass"), Role = "receptionist" },
                new LoginModel { Id = 3, Username = "manager", PasswordHash = BCrypt.Net.BCrypt.HashPassword("managerpass"), Role = "manager" }
            };
        }

        public LoginModel GetByUsername(string username)
        {
            try
            {
                return _users.SingleOrDefault(u => u.Username == username);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the user.", ex);
            }
        }
    }
}
