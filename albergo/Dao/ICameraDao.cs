using System.Collections.Generic;
using albergo.Models;
using Albergo.Services;

namespace albergo.DAO
{
    public interface ICameraDao
    {
        IEnumerable<AuthService> GetAll();
        AuthService GetById(int id);
        void Add(AuthService camera);
        void Update(AuthService camera);
        void Delete(int id);
    }
}