using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using SitoVetrina.Context;

namespace SitoVetrina.Models.ProdottoViewModels
{
    public class VisualizzaCarrelloViewModel
    {
        public string testoRicerca { get; set; }
        public List<Prodotto> ListProdotti { get; set; }
        public string Quantità { get; set; }
        public void InviaProdotti(List<Prodotto> prodotti)
        {
            ListProdotti = prodotti;
        }
    }
}
