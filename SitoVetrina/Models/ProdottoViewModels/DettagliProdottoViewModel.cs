using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using SitoVetrina.Context;
using SitoVetrina.Models.DbModels;

namespace SitoVetrina.Models.ProdottoViewModels
{
    public class DettagliProdottoViewModel
    {
        public Prodotto prodotto = new Prodotto();
        public string CodiceProdotto { get; set; }
        public string alert { get; set; }
        public string testoRicerca { get; set; }
        public InputModel Input { get; set; }
    }
}
