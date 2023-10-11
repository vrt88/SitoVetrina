using Microsoft.EntityFrameworkCore;
using SitoVetrina.Context;
using SitoVetrina.Models.DbModels;

namespace SitoVetrina.Contracts
{
    public interface IProdottoRepository
    {
        public List<Prodotto> VisualizzaProdotti(int pagina);
        public List<Prodotto> VisualizzaProdotti(string parametroRicerca,int pagina);
        public string CreaProdotto(string nome, string descrizione, decimal prezzo, string nomeImmagine);
        public Prodotto DettagliProdotto(string codiceProdotto);
        public string ModificaProdotto(string codiceProdotto, string nome, string descrizione, decimal prezzo, string nomeImmagine);
        public string EliminaProdotto(string codiceProdotto);
        public List<Prodotto> VisualizzaProdottiCarrello(string idUser);
        public List<Prodotto> VisualizzaProdottiCarrello(string idUser, string idProdotto);
        public string AggiungiProdottoCarrello(string idUser, string idProdotto);
        public string AggiornaQuantitàProdotto(string idUser, string idProdotto, int quantità);
        public string EliminaProdottoCarrello(string idUser, string idProdotto);
        public string CompraProdottiCarrello(string idUser);
    }
}
