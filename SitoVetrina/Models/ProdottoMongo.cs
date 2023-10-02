using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SitoVetrina.Models
{
    public class ProdottoMongo
    {
        public ProdottoMongo(string _id="",string nome="",decimal prezzo=0,string immagine="", string descrizione = "", int quantità=0)
        {
            if(_id!="")
            {
                this._id = new ObjectId(_id);
            }
            this.Nome = nome.Replace('"', '\'');
            this.Descrizione = descrizione.Replace('"', '\'');
            this.Prezzo = prezzo;
            this.Immagine = immagine;
            this.Quantità = quantità;
        }
        public ProdottoMongo()
        {
        }
        public ObjectId _id { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public decimal Prezzo { get; set; }
        public string Immagine { get; set; }
        public int Quantità { get; set; }
    }
}
