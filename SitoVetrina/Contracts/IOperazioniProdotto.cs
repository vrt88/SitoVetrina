using SitoVetrina.Context;
using SitoVetrina.Models.DbModels;

namespace SitoVetrina.Contracts
{
    public interface IOperazioniProdotto
    {
        public List<Prodotto> VisualizzaProdotti(DapperContext context);
        public List<Prodotto> VisualizzaProdotti( DapperContext context, string parametroRicerca);
        public string CreaProdotto(DapperContext context, string nome, string descrizione, string prezzo, string nomeImmagine);
        public Prodotto DettagliProdotto(DapperContext context, string codiceProdotto);
        public string ModificaProdotto(DapperContext context, string codiceProdotto, string nome, string descrizione, string prezzo, string nomeImmagine);
        public string EliminaProdotto(DapperContext context, string codiceProdotto);
    }
}
