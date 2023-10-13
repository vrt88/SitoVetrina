using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SitoVetrina.Context;
using SitoVetrina.Contracts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MongoDB.Driver;
using SitoVetrina.Models.DbModels;

namespace SitoVetrina.Models.Operazioni
{
    public class OperazioniProdotto : IOperazioniProdotto
    {
        public List<Prodotto> VisualizzaProdotti(DapperContext context)
        {
            string query;
            query = $"SELECT CodiceProdotto,Nome,Prezzo,Immagine FROM Prodotti";

            using (var connection = context.CreateConnection())
            {
                IEnumerable<Prodotto> prodotti = connection.Query<Prodotto>(query);
                return prodotti.ToList();
            }
        }
        public List<Prodotto> VisualizzaProdotti(DapperContext context, string parametroRicerca)
        {
            FormattableString formattableQuery;
            formattableQuery = $"SELECT CodiceProdotto,Nome,Prezzo,Immagine FROM Prodotti WHERE Nome LIKE CONCAT('%',@Nome,'%')";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "Nome", parametroRicerca } };

            string query = formattableQuery.ToString();

            using (var connection = context.CreateConnection())
            {
                IEnumerable<Prodotto> prodotti = connection.Query<Prodotto>(query, parameters);
                return prodotti.ToList();
            }
        }
        public string CreaProdotto(DapperContext context, string nome, string descrizione, string prezzo, string nomeImmagine)
        {
            try
            {
                FormattableString formattableQuery;
                formattableQuery = $"INSERT INTO Prodotti(Nome,Descrizione,Prezzo,Immagine) VALUES (@Nome,@Descrizione,@Prezzo,@Immagine);";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "Nome", nome }, { "Descrizione", descrizione }, { "Prezzo", prezzo }, { "Immagine", nomeImmagine } };

                string query = formattableQuery.ToString();
                using (var connection = context.CreateConnection())
                {
                    connection.Query(query, parameters);
                }

                FormattableString formattableQuery2;
                formattableQuery2 = $"SELECT CodiceProdotto FROM Prodotti WHERE Immagine=@Immagine;";

                Dictionary<string, object> parameters2 = new Dictionary<string, object>() { { "Immagine", nomeImmagine } };

                string query2 = formattableQuery2.ToString();
                string c;
                using (var connection = context.CreateConnection())
                {
                    c = connection.QueryFirst<Guid>(query2, parameters2).ToString();
                }
                return c;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public Prodotto DettagliProdotto(DapperContext context, string codiceProdotto)
        {
                FormattableString formattableQuery;
                formattableQuery = $"SELECT CodiceProdotto,Nome,Descrizione,Prezzo,Immagine FROM Prodotti WHERE Prodotti.CodiceProdotto=@CodiceProdotto;";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "CodiceProdotto", codiceProdotto.ToUpper() } };

                string query = formattableQuery.ToString();
                using (var connection = context.CreateConnection())
                {
                    Prodotto prodotto = connection.QuerySingle<Prodotto>(query, parameters);
                    return prodotto;
                }
        }
        public string ModificaProdotto(DapperContext context, string codiceProdotto, string nome, string descrizione, string prezzo, string nomeImmagine)
        {
            try
            {
                FormattableString formattableQuery;
                formattableQuery = $"UPDATE Prodotti SET Nome = @Nome,Descrizione=@Descrizione,Prezzo=@Prezzo,Immagine=@Immagine WHERE Prodotti.CodiceProdotto=@CodiceProdotto";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "Nome", nome }, { "Descrizione", descrizione }, { "Prezzo", prezzo }, { "Immagine", nomeImmagine }, { "CodiceProdotto", codiceProdotto.ToUpper() } };

                string query = formattableQuery.ToString();
                using (var connection = context.CreateConnection())
                {
                    connection.Query(query, parameters);
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string EliminaProdotto(DapperContext context, string codiceProdotto)
        {
            try
            {
                FormattableString formattableQuery;
                formattableQuery = $"DELETE FROM Prodotti WHERE CodiceProdotto=@CodiceProdotto";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "CodiceProdotto", codiceProdotto.ToUpper() } };

                string query = formattableQuery.ToString();
                using (var connection = context.CreateConnection())
                {
                    connection.Query(query, parameters);
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
