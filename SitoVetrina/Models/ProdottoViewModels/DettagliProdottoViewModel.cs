using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using SitoVetrina.Context;

namespace SitoVetrina.Models.ProdottoViewModels
{
    public class DettagliProdottoViewModel
    {
        public ProdottoMongo prodotto = new ProdottoMongo();
        public string CodiceProdotto { get; set; }
        public string alert { get; set; }
        public string testoRicerca { get; set; }
        public InputModel Input { get; set; }
    }
}
