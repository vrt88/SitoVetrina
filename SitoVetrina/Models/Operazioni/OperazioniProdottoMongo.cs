using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SitoVetrina.Context;
using SitoVetrina.Contracts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MongoDB.Driver;
using MongoDB.Bson;

namespace SitoVetrina.Models.Operazioni
{
    public class OperazioniProdottoMongo 
    {
        public List<ProdottoMongo> VisualizzaProdotti(MongoDBContext context)
        {
            var database=context.TakeDatabase();
            IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
            var fil = Builders<ProdottoMongo>.Filter.Empty;
            List<ProdottoMongo>prodotti = prodottiCollection.Find(fil).ToList();
            return prodotti;
        }
        public List<ProdottoMongo> VisualizzaProdotti(MongoDBContext context, string parametroRicerca)
        {
            var database = context.TakeDatabase();
            IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
            var fil = Builders<ProdottoMongo>.Filter.Eq("Nome",parametroRicerca);
            List<ProdottoMongo> prodotti = prodottiCollection.Find(fil).ToList();
            return prodotti;
        }
        public string CreaProdotto(MongoDBContext context, string nome, string descrizione, string prezzo, string nomeImmagine)
        {
            try
            {
                var database = context.TakeDatabase();
                IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
                ProdottoMongo prodotto = new ProdottoMongo("",nome,Convert.ToDecimal(prezzo)/100, nomeImmagine,descrizione); 
                prodottiCollection.InsertOne(prodotto);
                var fil = Builders<ProdottoMongo>.Filter.Eq("Immagine", nomeImmagine);
                string codiceProdotto = prodottiCollection.Find(fil).First()._id.ToString();
                return codiceProdotto;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public ProdottoMongo DettagliProdotto(MongoDBContext context, string codiceProdotto)
        {
            try
            {
                ObjectId id = new ObjectId(codiceProdotto);
                var database = context.TakeDatabase();
                IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
                var fil = Builders<ProdottoMongo>.Filter.Eq("_id", id);
                ProdottoMongo prodotto = prodottiCollection.Find(fil).First();
                return prodotto;
            }
            catch
            {
                return new ProdottoMongo();
            }
        }
        public string ModificaProdotto(MongoDBContext context, string codiceProdotto, string nome, string descrizione, string prezzo, string nomeImmagine)
        {
            try
            {
                ObjectId id = new ObjectId(codiceProdotto);
                var database = context.TakeDatabase();
                IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
                var fil = Builders<ProdottoMongo>.Filter.Eq("_id", id);
                ProdottoMongo prodotto= new ProdottoMongo(codiceProdotto, nome, Convert.ToDecimal(prezzo), nomeImmagine, descrizione);
                prodottiCollection.ReplaceOne(fil,prodotto);
                return "Nessun errore";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string EliminaProdotto(MongoDBContext context, string codiceProdotto)
        {
            try
            {
                ObjectId id = new ObjectId(codiceProdotto);
                var database = context.TakeDatabase();
                IMongoCollection<ProdottoMongo> prodottiCollection = database.GetCollection<ProdottoMongo>("Prodotti");
                var fil = Builders<ProdottoMongo>.Filter.Eq("_id", id);      
                prodottiCollection.DeleteOne(fil);
                return "Nessun errore";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
