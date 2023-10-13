using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SitoVetrina.Models.DbModels
{
    public abstract class Prodotto
    {
        public abstract string Nome { get; set; }
        public abstract string Descrizione { get; set; }
        public abstract decimal Prezzo { get; set; }
        public abstract string Immagine { get; set; }
        public abstract int Quantità { get; set; }
        public abstract string RitornaCodiceProdotto();
    }
}
