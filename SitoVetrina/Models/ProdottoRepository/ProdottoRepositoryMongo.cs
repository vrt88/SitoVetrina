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
            IMongoCollection<Prodotto> prodottiCollection = database.GetCollection<Prodotto>("Prodotti");
            FilterDefinition<Prodotto> fil = Builders<Prodotto>.Filter.Empty;
            List<Prodotto> prodotti = prodottiCollection.Find(fil).Skip(pagina * 16).Limit(16).ToList();
            return prodotti;
        }
        public List<Prodotto> VisualizzaProdotti(string parametroRicerca, int pagina)
        {
            IMongoDatabase database = _context.TakeDatabase();
            IMongoCollection<Prodotto> prodottiCollection = database.GetCollection<Prodotto>("Prodotti");
            FilterDefinition<Prodotto> fil = Builders<Prodotto>.Filter.Eq("Nome", "*" + parametroRicerca + "*");
            List<Prodotto> prodotti = prodottiCollection.Find(fil).Skip(pagina * 16).Limit((pagina + 1) * 16).ToList();
            return prodotti;
        }
        public string CreaProdotto(string nome, string descrizione, decimal prezzo, string nomeImmagine)
        {
            try
            {
                IMongoDatabase database = _context.TakeDatabase();
                IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
                ProdottoMongo prodotto = new ProdottoMongo("", nome, prezzo, nomeImmagine, descrizione);
                prodottiCollection.InsertOne(prodotto);
                FilterDefinition<ProdottoMongo> fil = Builders<ProdottoMongo>.Filter.Eq("Immagine", nomeImmagine);
                string codiceProdotto = prodottiCollection.Find(fil).First()._id.ToString();
                return codiceProdotto;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public Prodotto DettagliProdotto(string codiceProdotto)
        {
            try
            {
                ObjectId id = new ObjectId(codiceProdotto);
                IMongoDatabase database = _context.TakeDatabase();
                IMongoCollection<Prodotto> prodottiCollection = database.GetCollection<Prodotto>("Prodotti");
                FilterDefinition<Prodotto> fil = Builders<Prodotto>.Filter.Eq("_id", id);
                Prodotto prodotto = prodottiCollection.Find(fil).First();
                return prodotto;
            }
            catch
            {
                return new Prodotto();
            }
        }
        public string ModificaProdotto(string codiceProdotto, string nome, string descrizione, decimal prezzo, string nomeImmagine)
        {
            try
            {
                ObjectId id = new ObjectId(codiceProdotto);
                IMongoDatabase database = _context.TakeDatabase();
                IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
                FilterDefinition<ProdottoMongo> fil = Builders<ProdottoMongo>.Filter.Eq("_id", id);
                ProdottoMongo prodotto = new ProdottoMongo(codiceProdotto, nome, prezzo, nomeImmagine, descrizione);
                prodottiCollection.ReplaceOne(fil, prodotto);
                return "Nessun errore";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string EliminaProdotto(string codiceProdotto)
        {
            try
            {
                ObjectId id = new ObjectId(codiceProdotto);
                IMongoDatabase database = _context.TakeDatabase();
                IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
                FilterDefinition<ProdottoMongo> fil = Builders<ProdottoMongo>.Filter.Eq("_id", id);
                prodottiCollection.DeleteOne(fil);
                return "Nessun errore";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public List<Prodotto> VisualizzaProdottiCarrello(string idUser)
        {
            try
            {
                IMongoDatabase database = _context.TakeDatabase();
                ObjectId id = new ObjectId(idUser.Replace("-", ""));
                IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

                FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", id);
                ProdottoCarrello Carrello = carrelloCollection.Find(fil).First();
                return Carrello.Prodotti;
            }
            catch
            {
                return new List<Prodotto>();
            }
        }
        public List<Prodotto> VisualizzaProdottiCarrello(string idUser, string idProdotto)
        {
            try
            {
                IMongoDatabase database = _context.TakeDatabase();
                ObjectId id1 = new ObjectId(idUser.Replace("-", ""));
                ObjectId id2 = new ObjectId(idProdotto.Replace("-", ""));
                IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

                FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", id1) & Builders<ProdottoCarrello>.Filter.Eq("Prodotti._id", id2);
                ProdottoCarrello Carrello = carrelloCollection.Find(fil).First();
                return Carrello.Prodotti;
            }
            catch
            {
                return new List<Prodotto>();
            }
        }
        public string AggiungiProdottoCarrello(string idUser, string idProdotto)
        {
            try
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
                    Prodotto prodotto = DettagliProdotto(idProdotto);
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

                return "Prodotto aggiunto al carrello";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string AggiornaQuantitàProdotto(string idUser, string idProdotto, int quantità)
        {
            try
            {
                IMongoDatabase database = _context.TakeDatabase();
                IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

                FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", new ObjectId(idUser.Replace("-", ""))) & Builders<ProdottoCarrello>.Filter.Eq("Prodotti._id", new ObjectId(idProdotto.Replace("-", "")));
                UpdateDefinition<ProdottoCarrello> update = Builders<ProdottoCarrello>.Update.Set("Prodotti.$.Quantità", quantità);

                carrelloCollection.UpdateOne(fil, update);
                return "Prodotto aggiornato";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string EliminaProdottoCarrello( string idUser, string idProdotto)
        {
            try
            {
                IMongoDatabase database = _context.TakeDatabase();

                ObjectId id1 = new ObjectId(idUser.Replace("-", ""));
                ObjectId id2 = new ObjectId(idProdotto.Replace("-", ""));

                IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

                FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", id1);
                UpdateDefinition<ProdottoCarrello> update = Builders<ProdottoCarrello>.Update.PullFilter(carrello => carrello.Prodotti, prodotto => prodotto._id == id2);

                carrelloCollection.UpdateOne(fil, update);
                return "Prodotto eliminato";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string CompraProdottiCarrello(string idUser)
        {
            try
            {
                ObjectId id = new ObjectId(idUser.Replace("-", ""));
                IMongoDatabase database = _context.TakeDatabase();
                IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Carrello");

                FilterDefinition<ProdottoMongo> fil = Builders<ProdottoMongo>.Filter.Eq("_id", id);
                prodottiCollection.DeleteOne(fil);
                return "Prodotti Comprati";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
