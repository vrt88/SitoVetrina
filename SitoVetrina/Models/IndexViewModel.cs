using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SitoVetrina.Areas.Identity.Data;
using SitoVetrina.Contracts;
using System.Configuration;
using System.Security.Policy;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;

namespace SitoVetrina.Models
{
    public class IndexViewModel 
    {
        public string testoRicerca { get; set; }
        public int NumeroPagina { get; set; }
        public List<ProdottoMongo> ListProdotti { get; set; }
        public IndexViewModel(int numeroPagina)
        {
            this.NumeroPagina = numeroPagina;
        }
        public void InviaProdotti(List<ProdottoMongo> prodotti)
        {
            ListProdotti = prodotti;
        }
    }
}
