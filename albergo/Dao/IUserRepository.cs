using albergo.Models;

namespace Albergo.Repositories
{
    public interface IUserRepository
    {
        LoginModel GetByUsername(string username);
    }
}