using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SitoVetrina.Models.DbModels
{
    public class Prodotto
    {
        public Prodotto(string _id = "", string nome = "", decimal prezzo = 0, string immagine = "", string descrizione = "", int quantità = 1)
        {
            if (_id != "")
            {
                this._id = new ObjectId(_id);
            }
            Nome = nome.Replace('"', '\'');
            Descrizione = descrizione.Replace('"', '\'');
            Prezzo = prezzo;
            Immagine = immagine;
            Quantità = quantità;
        }
        public Prodotto(Guid codiceProdotto, string nome = "", decimal prezzo = 0, string immagine = "", string descrizione = "", int quantità = 1)
        {
            CodiceProdotto = codiceProdotto;    
            Nome = nome.Replace('"', '\'');
            Descrizione = descrizione.Replace('"', '\'');
            Prezzo = prezzo;
            Immagine = immagine;
            Quantità = quantità;
        }
        public Prodotto()
        {
        }
        public ObjectId _id { get; set; }
        public Guid CodiceProdotto { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public decimal Prezzo { get; set; }
        public string Immagine { get; set; }
        public int Quantità { get; set; }
    }
}
