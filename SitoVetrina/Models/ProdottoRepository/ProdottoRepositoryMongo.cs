using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SitoVetrina.Context;
using SitoVetrina.Contracts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MongoDB.Driver;
using MongoDB.Bson;
using SitoVetrina.Models.DbModels;

namespace SitoVetrina.Models.ProdottoRepository
{
    public class ProdottoRepositoryMongo : IProdottoRepository
    {
        private readonly MongoDBContext _context;
        public ProdottoRepositoryMongo(IConfiguration configuration)
        {
            _context = new MongoDBContext(configuration);
        }
        public List<Prodotto> VisualizzaProdotti(int pagina)
        {
            IMongoDatabase database = _context.TakeDatabase();
            IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
            FilterDefinition<ProdottoMongo> fil = Builders<ProdottoMongo>.Filter.Empty;
            List<ProdottoMongo> prodottiMongo = prodottiCollection.Find(fil).Skip(pagina * 16).Limit(16).ToList();
            List<Prodotto> prodotti= prodottiMongo.ToList<Prodotto>();
            return prodotti;
        }
        public List<Prodotto> VisualizzaProdotti(string parametroRicerca, int pagina)
        {
            IMongoDatabase database = _context.TakeDatabase();
            IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
            FilterDefinition<ProdottoMongo> fil = Builders<ProdottoMongo>.Filter.Eq("Nome", "*" + parametroRicerca + "*");
            List<ProdottoMongo> prodottiMongo = prodottiCollection.Find(fil).Skip(pagina * 16).Limit((pagina + 1) * 16).ToList();
            List<Prodotto> prodotti = prodottiMongo.ToList<Prodotto>();
            return prodotti;
        }
        public string CreaProdotto(string nome, string descrizione, decimal prezzo, string nomeImmagine)
        {

            IMongoDatabase database = _context.TakeDatabase();
            IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
            ProdottoMongo prodotto = new ProdottoMongo("", nome, prezzo, nomeImmagine, descrizione);
            prodottiCollection.InsertOne(prodotto);
            FilterDefinition<ProdottoMongo> fil = Builders<ProdottoMongo>.Filter.Eq("Immagine", nomeImmagine);
            string codiceProdotto = prodottiCollection.Find(fil).First()._id.ToString();
            return codiceProdotto;

        }
        public Prodotto DettagliProdotto(string codiceProdotto)
        {
            ObjectId id = new ObjectId(codiceProdotto);
            IMongoDatabase database = _context.TakeDatabase();
            IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
            FilterDefinition<ProdottoMongo> fil = Builders<ProdottoMongo>.Filter.Eq("_id", id);
            Prodotto prodotto = prodottiCollection.Find(fil).First();
            return prodotto;
        }
        public void ModificaProdotto(string codiceProdotto, string nome, string descrizione, decimal prezzo, string nomeImmagine)
        {

            ObjectId id = new ObjectId(codiceProdotto);
            IMongoDatabase database = _context.TakeDatabase();
            IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
            FilterDefinition<ProdottoMongo> fil = Builders<ProdottoMongo>.Filter.Eq("_id", id);
            ProdottoMongo prodotto = new ProdottoMongo(codiceProdotto, nome, prezzo, nomeImmagine, descrizione);
            prodottiCollection.ReplaceOne(fil, prodotto);

        }
        public void EliminaProdotto(string codiceProdotto)
        {

            ObjectId id = new ObjectId(codiceProdotto);
            IMongoDatabase database = _context.TakeDatabase();
            IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
            FilterDefinition<ProdottoMongo> fil = Builders<ProdottoMongo>.Filter.Eq("_id", id);
            prodottiCollection.DeleteOne(fil);

        }
        public List<Prodotto> VisualizzaProdottiCarrello(string idUser)
        {
            IMongoDatabase database = _context.TakeDatabase();
            ObjectId id = new ObjectId(idUser.Replace("-", ""));
            IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

            FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", id);
            ProdottoCarrello Carrello = carrelloCollection.Find(fil).FirstOrDefault();
            if (Carrello != null)
            {
                return Carrello.Prodotti.ToList<Prodotto>();
            }
            else
            {
                return new List<Prodotto>();
            }
        }
        public List<Prodotto> VisualizzaProdottiCarrello(string idUser, string idProdotto)
        {
            IMongoDatabase database = _context.TakeDatabase();
            ObjectId id1 = new ObjectId(idUser.Replace("-", ""));
            ObjectId id2 = new ObjectId(idProdotto);
            IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

            FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", id1) & Builders<ProdottoCarrello>.Filter.Eq("Prodotti._id", id2);
            ProdottoCarrello Carrello = carrelloCollection.Find(fil).FirstOrDefault();
            if(Carrello!= null)
            {
                return Carrello.Prodotti.ToList<Prodotto>();
            }
            else
            {
                return new List<Prodotto>();
            }
        }
        public void AggiungiProdottoCarrello(string idUser, string idProdotto)
        {


            IMongoDatabase database = _context.TakeDatabase();
            IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

            if (VisualizzaProdottiCarrello(idUser, idProdotto).Count != 0)
            {
                FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", new ObjectId(idUser.Replace("-", ""))) & Builders<ProdottoCarrello>.Filter.Eq("Prodotti._id", new ObjectId(idProdotto));
                UpdateDefinition<ProdottoCarrello> update = Builders<ProdottoCarrello>.Update.Inc("Prodotti.$.Quantità", 1);

                carrelloCollection.UpdateOne(fil, update);
            }
            else
            {
                ProdottoMongo prodotto = (ProdottoMongo)DettagliProdotto(idProdotto);
                FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", new ObjectId(idUser.Replace("-", "")));
                ProdottoCarrello carrello = carrelloCollection.Find(fil).FirstOrDefault();
                if ((carrello != null) && (carrello.Prodotti != null))
                {
                    carrello.Prodotti.Add(prodotto);
                    carrelloCollection.ReplaceOne(fil, carrello);
                }
                else
                {
                    ProdottoCarrello carrelloNuovo = new ProdottoCarrello(idUser.Replace("-", ""));
                    carrelloNuovo.Prodotti.Add(prodotto);
                    carrelloCollection.InsertOne(carrelloNuovo);
                }
            }

        }
        public void AggiornaQuantitàProdotto(string idUser, string idProdotto, int quantità)
        {

            IMongoDatabase database = _context.TakeDatabase();
            IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

            FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", new ObjectId(idUser.Replace("-", ""))) & Builders<ProdottoCarrello>.Filter.Eq("Prodotti._id", new ObjectId(idProdotto.Replace("-", "")));
            UpdateDefinition<ProdottoCarrello> update = Builders<ProdottoCarrello>.Update.Set("Prodotti.$.Quantità", quantità);

            carrelloCollection.UpdateOne(fil, update);

        }
        public void EliminaProdottoCarrello(string idUser, string idProdotto)
        {

            IMongoDatabase database = _context.TakeDatabase();

            ObjectId id1 = new ObjectId(idUser.Replace("-", ""));
            ObjectId id2 = new ObjectId(idProdotto);

            IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

            FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", id1);
            UpdateDefinition<ProdottoCarrello> update = Builders<ProdottoCarrello>.Update.PullFilter(carrello => carrello.Prodotti,prodotto => prodotto._id== id2);

            carrelloCollection.UpdateOne(fil, update);

        }
        public void CompraProdottiCarrello(string idUser)
        {

            ObjectId id = new ObjectId(idUser.Replace("-", ""));
            IMongoDatabase database = _context.TakeDatabase();
            IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Carrello");

            FilterDefinition<ProdottoMongo> fil = Builders<ProdottoMongo>.Filter.Eq("_id", id);
            prodottiCollection.DeleteOne(fil);
        }
    }
}
