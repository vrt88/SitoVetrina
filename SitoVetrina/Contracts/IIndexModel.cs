using SitoVetrina.Models.DbModels;

namespace SitoVetrina.Contracts
{
    public interface IIndexModel
    {
        public void InviaProdotti(List<Prodotto> prodotti);
    }
}
