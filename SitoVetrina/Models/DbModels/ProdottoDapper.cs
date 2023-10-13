using MongoDB.Bson;
using System.Security.Cryptography;

namespace SitoVetrina.Models.DbModels
{
    public class ProdottoDapper : Prodotto
    {
        public ProdottoDapper(Guid codiceProdotto, string nome = "", decimal prezzo = 0, string immagine = "", string descrizione = "", int quantità = 1)
        {
            CodiceProdotto = codiceProdotto;
            Nome = nome.Replace('"', '\'');
            Descrizione = descrizione.Replace('"', '\'');
            Prezzo = prezzo;
            Immagine = immagine;
            Quantità = quantità;
        }
        public ProdottoDapper()
        {
        }
        public override string RitornaCodiceProdotto()
        {
            return CodiceProdotto.ToString();
        }
        public Guid CodiceProdotto { get; set; }
        public override string Nome { get; set; }
        public override string Descrizione { get; set; }
        public override decimal Prezzo { get; set; }
        public override string Immagine { get; set; }
        public override int Quantità { get; set; }
    }
}
