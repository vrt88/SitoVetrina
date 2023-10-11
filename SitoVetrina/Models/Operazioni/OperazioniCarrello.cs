using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SitoVetrina.Context;
using SitoVetrina.Contracts;
using SitoVetrina.Models.DbModels;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SitoVetrina.Models.Operazioni
{
    public class OperazioniCarrello
    {
        public List<Prodotto> VisualizzaProdottiCarrello(DapperContext context, string idUser)
        {
            FormattableString formattableQuery;
            formattableQuery = $"SELECT CodiceProdotto,Nome,Prezzo,Immagine,Quantità FROM Carrello,Prodotti WHERE (Prodotti.CodiceProdotto=Carrello.IdProdotto) AND (Carrello.IdUser=@IdUser);";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "IdUser", idUser } };

            string query = formattableQuery.ToString();

            using (var connection = context.CreateConnection())
            {
                IEnumerable<Prodotto> prodotti = connection.Query<Prodotto>(query, parameters);
                return prodotti.ToList();
            }
        }
        public List<Prodotto> VisualizzaProdottiCarrello(DapperContext context, string idUser, string idProdotto)
        {
            FormattableString formattableQuery;
            formattableQuery = $"SELECT CodiceProdotto,Nome,Prezzo,Immagine FROM Carrello,Prodotti WHERE (Prodotti.CodiceProdotto=Carrello.IdProdotto) AND (Carrello.IdUser=@IdUser) AND (Prodotti.CodiceProdotto=@idProdotto);";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "IdUser", idUser }, { "IdProdotto", idProdotto } };

            string query = formattableQuery.ToString();

            using (var connection = context.CreateConnection())
            {
                IEnumerable<Prodotto> prodotti = connection.Query<Prodotto>(query, parameters);
                return prodotti.ToList();
            }
        }
        public string AggiungiProdottoCarrello(DapperContext context, string idUser, string idProdotto)
        {
            try
            {
                FormattableString formattableQuery;

                if (VisualizzaProdottiCarrello(context, idUser, idProdotto).Count != 0)
                {
                    formattableQuery = $"UPDATE Carrello SET Carrello.Quantità=Carrello.Quantità+1 WHERE (Carrello.IdUser=@IdUser) AND (Carrello.IdProdotto=@idProdotto);";
                }
                else
                {
                    formattableQuery = $"INSERT INTO Carrello(IdUser,IdProdotto) VALUES (@IdUser,@IdProdotto);";
                }
                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "IdUser", idUser }, { "IdProdotto", idProdotto } };

                string query = formattableQuery.ToString();
                using (var connection = context.CreateConnection())
                {
                    connection.Query(query, parameters);
                }
                return "Prodotto aggiunto al carrello";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string AggiornaQuantitàProdotto(DapperContext context, string idUser, string idProdotto, int quantità)
        {
            try
            {
                FormattableString formattableQuery;

                formattableQuery = $"UPDATE Carrello SET Carrello.Quantità=@Quantità WHERE (Carrello.IdUser=@IdUser) AND (Carrello.IdProdotto=@idProdotto);";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "IdUser", idUser }, { "IdProdotto", idProdotto }, { "Quantità", quantità } };

                string query = formattableQuery.ToString();
                using (var connection = context.CreateConnection())
                {
                    connection.Query(query, parameters);
                }
                return "Prodotto aggiornato";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string EliminaProdottoCarrello(DapperContext context, string idUser, string idProdotto)
        {
            try
            {
                FormattableString formattableQuery;
                formattableQuery = $"DELETE FROM Carrello WHERE idProdotto=@idProdotto AND idUser=@idUser";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "idProdotto", idProdotto.ToUpper() }, { "idUser", idUser } };

                string query = formattableQuery.ToString();
                using (var connection = context.CreateConnection())
                {
                    connection.QueryFirst(query, parameters);
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string CompraProdottiCarrello(DapperContext context, string idUser)
        {
            try
            {
                FormattableString formattableQuery;
                formattableQuery = $"DELETE FROM Carrello WHERE idUser=@idUser";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "idUser", idUser } };

                string query = formattableQuery.ToString();
                using (var connection = context.CreateConnection())
                {
                    connection.QueryFirst(query, parameters);
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
