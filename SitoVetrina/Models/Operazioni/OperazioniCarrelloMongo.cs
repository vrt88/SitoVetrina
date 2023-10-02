using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using SitoVetrina.Context;
using SitoVetrina.Contracts;
using System.Collections.Generic;
using System.Security.Permissions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SitoVetrina.Models.Operazioni
{
    public class OperazioniCarrelloMongo
    {
        public List<ProdottoMongo> VisualizzaProdottiCarrello(MongoDBContext context, string idUser)
        {
            try
            {
                IMongoDatabase database = context.TakeDatabase();
                ObjectId id = new ObjectId(idUser.Replace("-", ""));
                IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

                FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", id);
                ProdottoCarrello Carrello = carrelloCollection.Find(fil).First();
                return Carrello.Prodotti;
            }
            catch
            {
                return new List<ProdottoMongo>();
            }
        }
        public List<ProdottoMongo> VisualizzaProdottiCarrello(MongoDBContext context, string idUser, string idProdotto)
        {
            try
            {
                IMongoDatabase database = context.TakeDatabase();
                ObjectId id1 = new ObjectId(idUser.Replace("-", ""));
                ObjectId id2 = new ObjectId(idProdotto);
                IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

                FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", id1) & Builders<ProdottoCarrello>.Filter.Eq("Prodotti._id", id2);
                ProdottoCarrello Carrello = carrelloCollection.Find(fil).First();
                return Carrello.Prodotti;
            }
            catch
            {
                return new List<ProdottoMongo>();
            }
        }
        public string AggiungiProdottoCarrello(MongoDBContext context, string idUser, string idProdotto)
        {
            try
            {
                OperazioniProdottoMongo operazioniProdotto= new OperazioniProdottoMongo();

                IMongoDatabase database = context.TakeDatabase();
                IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

                if (VisualizzaProdottiCarrello(context, idUser, idProdotto).Count != 0)
                {
                    FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", new ObjectId(idUser.Replace("-", ""))) & Builders<ProdottoCarrello>.Filter.Eq("Prodotti._id", new ObjectId(idProdotto));
                    UpdateDefinition<ProdottoCarrello> update = Builders<ProdottoCarrello>.Update.Inc("Prodotti.$.Quantità",1);

                    carrelloCollection.UpdateOne(fil, update);
                }
                else
                {
                    ProdottoMongo prodotto = operazioniProdotto.DettagliProdotto(context, idProdotto);
                    FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", new ObjectId(idUser.Replace("-", "")));
                    ProdottoCarrello carrello = carrelloCollection.Find(fil).FirstOrDefault();
                    if ((carrello!=null)&&(carrello.Prodotti != null))
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
        public string AggiornaQuantitàProdotto(MongoDBContext context, string idUser, string idProdotto, int quantità)
        {
            try
            {
                IMongoDatabase database = context.TakeDatabase();
                IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

                FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", new ObjectId(idUser.Replace("-", ""))) & Builders<ProdottoCarrello>.Filter.Eq("Prodotti._id", new ObjectId(idProdotto));
                UpdateDefinition<ProdottoCarrello> update = Builders<ProdottoCarrello>.Update.Set("Prodotti.$.Quantità", quantità);

                carrelloCollection.UpdateOne(fil, update);
                return "Prodotto aggiornato";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string EliminaProdottoCarrello(MongoDBContext context, string idUser, string idProdotto)
        {
            try
            {
                IMongoDatabase database = context.TakeDatabase();

                ObjectId id1 = new ObjectId(idUser.Replace("-", ""));
                ObjectId id2 = new ObjectId(idProdotto);

                IMongoCollection<ProdottoCarrello> carrelloCollection = database.GetCollection<ProdottoCarrello>("Carrello");

                FilterDefinition<ProdottoCarrello> fil = Builders<ProdottoCarrello>.Filter.Eq("_id", id1) ;
                UpdateDefinition<ProdottoCarrello> update = Builders<ProdottoCarrello>.Update.PullFilter(carrello => carrello.Prodotti, prodotto => prodotto._id == id2);

                carrelloCollection.UpdateOne(fil,update);
                return "Prodotto eliminato";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string CompraProdottiCarrello(MongoDBContext context, string idUser)
        {
            try
            {
                ObjectId id = new ObjectId(idUser);
                IMongoDatabase database = context.TakeDatabase();
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
