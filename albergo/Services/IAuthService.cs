using albergo.Models;  

namespace Albergo.Services
{
    public interface IAuthService
    {
        LoginModel Login(string username, string password);
    }
}