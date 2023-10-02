namespace SitoVetrina.Contracts
{
    public interface IOperazioniImmagine
    {
        string CreaImmagine(IWebHostEnvironment hostEnvironment, IFormFile immagine);
        void EliminaImmagine(string immagine);
        string GetUniqueFileName(string fileName);
    }
}