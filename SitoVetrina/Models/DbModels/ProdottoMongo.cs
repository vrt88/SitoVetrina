﻿using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SitoVetrina.Models.DbModels
{
    public class ProdottoMongo:Prodotto
    {
        public ProdottoMongo(string _id = "", string nome = "", decimal prezzo = 0, string immagine = "", string descrizione = "", int quantità = 1)
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
        public ProdottoMongo()
        {
        }
        public override string RitornaCodiceProdotto()
        {
            return _id.ToString();
        }
        public ObjectId _id { get; set; }
        public override string Nome { get; set; }
        public override string Descrizione { get; set; }
        public override decimal Prezzo { get; set; }
        public override string Immagine { get; set; }
        public override int Quantità { get; set; }
    }
}
