using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SitoVetrina.Context;
using SitoVetrina.Contracts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MongoDB.Driver;
using SitoVetrina.Models.DbModels;

namespace SitoVetrina.Models.ProdottoRepository
{
    public class ProdottoRepositoryDapper : IProdottoRepository
    {
        private readonly DapperContext _context;
        public ProdottoRepositoryDapper(IConfiguration configuration)
        {
            _context = new DapperContext(configuration);
        }
        public List<Prodotto> VisualizzaProdotti(int pagina)
        {
            string query = $"SELECT CodiceProdotto,Nome,Prezzo,Immagine FROM Prodotti";

            using (var connection = _context.CreateConnection())
            {
                IEnumerable<Prodotto> prodotti = connection.Query<Prodotto>(query);
                return prodotti.ToList();
            }
        }
        public List<Prodotto> VisualizzaProdotti(string parametroRicerca, int pagina)
        {
            FormattableString formattableQuery= $"SELECT CodiceProdotto,Nome,Prezzo,Immagine FROM Prodotti WHERE Nome LIKE CONCAT('%',@Nome,'%')";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "Nome", parametroRicerca } };

            string query = formattableQuery.ToString();

            using (var connection = _context.CreateConnection())
            {
                IEnumerable<Prodotto> prodotti = connection.Query<Prodotto>(query, parameters);
                return prodotti.ToList();
            }
        }
        public string CreaProdotto(string nome, string descrizione, decimal prezzo, string nomeImmagine)
        {
            try
            {
                FormattableString formattableQuery= $"INSERT INTO Prodotti(Nome,Descrizione,Prezzo,Immagine) VALUES (@Nome,@Descrizione,@Prezzo,@Immagine);";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "Nome", nome }, { "Descrizione", descrizione }, { "Prezzo", prezzo }, { "Immagine", nomeImmagine } };

                string query = formattableQuery.ToString();
                using (var connection = _context.CreateConnection())
                {
                    connection.Query(query, parameters);
                }

                FormattableString formattableQuery2=$"SELECT CodiceProdotto FROM Prodotti WHERE Immagine=@Immagine;";

                Dictionary<string, object> parameters2 = new Dictionary<string, object>() { { "Immagine", nomeImmagine } };

                string query2 = formattableQuery2.ToString();
                string c;
                using (var connection = _context.CreateConnection())
                {
                    c = connection.QueryFirst<Guid>(query2, parameters2).ToString();
                }
                return c;
            }
            catch
            {
                return "Errore";
            }
        }
        public Prodotto DettagliProdotto(string codiceProdotto)
        {
            FormattableString formattableQuery = $"SELECT CodiceProdotto,Nome,Descrizione,Prezzo,Immagine FROM Prodotti WHERE Prodotti.CodiceProdotto=@CodiceProdotto;";

            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "CodiceProdotto", codiceProdotto.ToUpper() } };

            string query = formattableQuery.ToString();
            using (var connection = _context.CreateConnection())
            {
                Prodotto prodotto = connection.QuerySingle<Prodotto>(query, parameters);
                return prodotto;
            }
        }
        public string ModificaProdotto(string codiceProdotto, string nome, string descrizione, decimal prezzo, string nomeImmagine)
        {
            try
            {
                FormattableString formattableQuery = $"UPDATE Prodotti SET Nome = @Nome,Descrizione=@Descrizione,Prezzo=@Prezzo,Immagine=@Immagine WHERE Prodotti.CodiceProdotto=@CodiceProdotto";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "Nome", nome }, { "Descrizione", descrizione }, { "Prezzo", prezzo }, { "Immagine", nomeImmagine }, { "CodiceProdotto", codiceProdotto.ToUpper() } };

                string query = formattableQuery.ToString();
                using (var connection = _context.CreateConnection())
                {
                    connection.Query(query, parameters);
                }
                return "Prodotto modificato";
            }
            catch
            {
                return "Errore";
            }
        }
        public string EliminaProdotto(string codiceProdotto)
        {
            try
            {
                FormattableString formattableQuery= $"DELETE FROM Prodotti WHERE CodiceProdotto=@CodiceProdotto";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "CodiceProdotto", codiceProdotto.ToUpper() } };

                string query = formattableQuery.ToString();
                using (var connection = _context.CreateConnection())
                {
                    connection.Execute(query, parameters);
                }
                return "Prodotto eliminato";
            }
            catch
            {
                return "Errore";
            }
        }
        public List<Prodotto> VisualizzaProdottiCarrello(string idUser)
        {
            FormattableString formattableQuery= $"SELECT CodiceProdotto,Nome,Prezzo,Immagine,Quantità FROM Carrello,Prodotti WHERE (Prodotti.CodiceProdotto=Carrello.IdProdotto) AND (Carrello.IdUser=@IdUser);";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "IdUser", idUser } };

            string query = formattableQuery.ToString();

            using (var connection = _context.CreateConnection())
            {
                IEnumerable<Prodotto> prodotti = connection.Query<Prodotto>(query, parameters);
                return prodotti.ToList();
            }
        }
        public List<Prodotto> VisualizzaProdottiCarrello(string idUser, string idProdotto)
        {
            FormattableString formattableQuery= $"SELECT CodiceProdotto,Nome,Prezzo,Immagine FROM Carrello,Prodotti WHERE (Prodotti.CodiceProdotto=Carrello.IdProdotto) AND (Carrello.IdUser=@IdUser) AND (Prodotti.CodiceProdotto=@idProdotto);";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "IdUser", idUser }, { "IdProdotto", idProdotto } };

            string query = formattableQuery.ToString();

            using (var connection = _context.CreateConnection())
            {
                IEnumerable<Prodotto> prodotti = connection.Query<Prodotto>(query, parameters);
                return prodotti.ToList();
            }
        }
        public string AggiungiProdottoCarrello(string idUser, string idProdotto)
        {
            try
            {
                FormattableString formattableQuery;

                if (VisualizzaProdottiCarrello(idUser, idProdotto).Count != 0)
                {
                    formattableQuery = $"UPDATE Carrello SET Carrello.Quantità=Carrello.Quantità+1 WHERE (Carrello.IdUser=@IdUser) AND (Carrello.IdProdotto=@idProdotto);";
                }
                else
                {
                    formattableQuery = $"INSERT INTO Carrello(IdUser,IdProdotto) VALUES (@IdUser,@IdProdotto);";
                }
                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "IdUser", idUser }, { "IdProdotto", idProdotto } };

                string query = formattableQuery.ToString();
                using (var connection = _context.CreateConnection())
                {
                    connection.Query(query, parameters);
                }
                return "Prodotto aggiunto al carrello";
            }
            catch
            {
                return "Errore";
            }
        }
        public string AggiornaQuantitàProdotto(string idUser, string idProdotto, int quantità)
        {
            try
            {
                FormattableString formattableQuery= $"UPDATE Carrello SET Carrello.Quantità=@Quantità WHERE (Carrello.IdUser=@IdUser) AND (Carrello.IdProdotto=@idProdotto);";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "IdUser", idUser }, { "IdProdotto", idProdotto }, { "Quantità", quantità } };

                string query = formattableQuery.ToString();
                using (var connection = _context.CreateConnection())
                {
                    connection.Query(query, parameters);
                }
                return "Prodotto aggiornato";
            }
            catch
            {
                return "Errore";
            }
        }
        public string EliminaProdottoCarrello(string idUser, string idProdotto)
        {
            try
            {
                FormattableString formattableQuery = $"DELETE FROM Carrello WHERE idProdotto=@idProdotto AND idUser=@idUser";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "idProdotto", idProdotto.ToUpper() }, { "idUser", idUser } };

                string query = formattableQuery.ToString();
                using (var connection = _context.CreateConnection())
                {
                    connection.Execute(query, parameters);
                }
                return "Prodotto Carrello Eliminato";
            }
            catch
            {
                return "Errore";
            }
        }
        public string CompraProdottiCarrello(string idUser)
        {
            try
            {
                FormattableString formattableQuery= $"DELETE FROM Carrello WHERE idUser=@idUser";

                Dictionary<string, object> parameters = new Dictionary<string, object>() { { "idUser", idUser } };

                string query = formattableQuery.ToString();
                using (var connection = _context.CreateConnection())
                {
                    connection.Execute(query, parameters);
                }
                return "Prodotto comprato";
            }
            catch
            {
                return "Errore";
            }
        }
    }
}
