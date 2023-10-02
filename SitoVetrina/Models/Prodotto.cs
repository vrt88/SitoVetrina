using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SitoVetrina.Models
{
    public class Prodotto
    {
        public Prodotto(Guid codiceProdotto,string nome="",decimal prezzo=0,string immagine="", string descrizione = "", int quantità=0)
        {
            this.CodiceProdotto = codiceProdotto;
            this.Nome = nome.Replace('"', '\'');
            this.Descrizione = descrizione.Replace('"', '\'');
            this.Prezzo = prezzo;
            this.Immagine = immagine;
            this.Quantità = quantità;
        }
        public Prodotto()
        {
        }
        public Guid CodiceProdotto { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public decimal Prezzo { get; set; }
        public string Immagine { get; set; }
        public int Quantità { get; set; }
    }
}
