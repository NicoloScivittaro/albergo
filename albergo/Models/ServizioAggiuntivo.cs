namespace albergo.Models
{
    public class ServizioAggiuntivo
    {
        public int Id { get; set; }
        public int PrenotazioneId { get; set; }
        public Prenotazione Prenotazione { get; set; }
        public DateTime Data { get; set; }
        public int Quantita { get; set; }
        public decimal Prezzo { get; set; }
        public string Descrizione { get; set; }
    }
}
