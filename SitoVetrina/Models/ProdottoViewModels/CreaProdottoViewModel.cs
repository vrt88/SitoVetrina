using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using SitoVetrina.Context;

namespace SitoVetrina.Models.ProdottoViewModels
{
    public class CreaProdottoViewModel
    {
        public InputModel Input { get; set; }

        public string alert { get; set; }
        public string testoRicerca { get; set; }
    }
}
