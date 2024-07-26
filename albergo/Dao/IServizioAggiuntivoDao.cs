using System.Collections.Generic;
using albergo.Models;

namespace albergo.DAO
{
    public interface IServizioAggiuntivoDao
    {
        IEnumerable<ServizioAggiuntivo> GetAll();
        ServizioAggiuntivo GetById(int id);
        void Add(ServizioAggiuntivo servizio);
        void Update(ServizioAggiuntivo servizio);
        void Delete(int id);
        IEnumerable<ServizioAggiuntivo> GetByPrenotazioneId(int prenotazioneId);
    }
}