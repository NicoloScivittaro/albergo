using System.Collections.Generic;
using albergo.Models;

namespace Albergo.DAO
{
    public interface IClienteDao
    {
        IEnumerable<Cliente> GetAll();
        Cliente GetById(int id);
        void Add(Cliente cliente);
        void Update(Cliente cliente);
        void Delete(int id);
    }
}