using Microsoft.Extensions.Hosting;
using SitoVetrina.Contracts;
using System.Diagnostics;

namespace SitoVetrina.Models.Operazioni
{
    public class OperazioniImmagine : IOperazioniImmagine
    {
        public string CreaImmagine(IWebHostEnvironment hostEnvironment, IFormFile immagine)
        {
            var uniqueFileName = GetUniqueFileName(immagine.FileName);
            var uploads = Path.Combine(hostEnvironment.WebRootPath, "images");
            var filePath = Path.Combine(uploads, uniqueFileName);
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            immagine.CopyTo(fileStream);
            fileStream.Close();
            return uniqueFileName;
        }
        public void EliminaImmagine(string immagine)
        {
            DirectoryInfo dir = new DirectoryInfo("wwwroot\\Images\\");
            FileInfo file = new FileInfo(dir.FullName + immagine);
            file.Delete();
        }
        public string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }
    }
}
