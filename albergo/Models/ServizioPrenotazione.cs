namespace albergo.Models
{
    public class ServizioPrenotazione
    {
        public int Id { get; set; }
        public int PrenotazioneId { get; set; }
        public int ServizioId { get; set; }
        public DateTime Data { get; set; }
        public int Quantita { get; set; }
        public decimal Prezzo { get; set; }
        public Servizio Servizio { get; set; }
    }
}