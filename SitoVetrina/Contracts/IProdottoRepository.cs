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
        public void ModificaProdotto(string codiceProdotto, string nome, string descrizione, decimal prezzo, string nomeImmagine);
        public void EliminaProdotto(string codiceProdotto);
        public List<Prodotto> VisualizzaProdottiCarrello(string idUser);
        public List<Prodotto> VisualizzaProdottiCarrello(string idUser, string idProdotto);
        public void AggiungiProdottoCarrello(string idUser, string idProdotto);
        public void AggiornaQuantitàProdotto(string idUser, string idProdotto, int quantità);
        public void EliminaProdottoCarrello(string idUser, string idProdotto);
        public void EliminaCarrello(string idUser);
    }
}
