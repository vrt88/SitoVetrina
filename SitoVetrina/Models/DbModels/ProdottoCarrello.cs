using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SitoVetrina.Models.DbModels
{
    public class ProdottoCarrello
    {
        public ProdottoCarrello(string _id = "", List<Prodotto> prodotti = null)
        {
            if (_id != "")
            {
                this._id = new ObjectId(_id);
            }
            if (prodotti == null)
            {
                Prodotti = new List<Prodotto>();
            }
            else
            {
                Prodotti = prodotti;
            }
        }
        public ProdottoCarrello()
        {
        }
        public ObjectId _id { get; set; }
        public List<Prodotto> Prodotti { get; set; }
    }
}
