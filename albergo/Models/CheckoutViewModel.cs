namespace albergo.Models
{
   
    public class CheckoutViewModel
    {
        public Prenotazione Prenotazione { get; set; }
        public decimal TotaleStanza { get; set; }
        public decimal TotaleServizi { get; set; }
        public decimal Totale { get; set; }
        public decimal ExtraSoggiorno { get; set; }
    }
}