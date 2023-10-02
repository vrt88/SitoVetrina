using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using SitoVetrina.Areas.Identity.Data;
using SitoVetrina.Context;
using SitoVetrina.Contracts;
using SitoVetrina.Models;
using SitoVetrina.Models.Operazioni;
using SitoVetrina.Models.ProdottoViewModels;
using System;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Cryptography.X509Certificates;

namespace SitoVetrina.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DapperContext _context;
        private readonly MongoDBContext _mongoContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, DapperContext context,UserManager<ApplicationUser> userManager,MongoDBContext mongoContext)
        {
            _logger = logger;
            _context = context;
            _userManager= userManager;
            _mongoContext= mongoContext;
        }
        public IActionResult Index()
        {
            IOperazioniProdotto operazioniProdotto= new OperazioniProdotto();
            OperazioniProdottoMongo operazioniProdottoMongo = new OperazioniProdottoMongo();
            OperazioniCarrelloMongo operazioniCarrello= new OperazioniCarrelloMongo();
            IndexViewModel indexModel ;
            List<ProdottoMongo> prodotti;
            string url = HttpContext.Request.GetDisplayUrl();
            string parametroRicerca;
            int numeroPagina=0;
            if (url.Contains("Avanti"))
            {
                numeroPagina = Convert.ToInt16(url.Substring(url.IndexOf("index/") + 6).Replace(",Avanti",""));
                numeroPagina++;
                indexModel = new IndexViewModel(numeroPagina);
            }
            else if (url.Contains("Indietro"))
            {
                numeroPagina = Convert.ToInt16(url.Substring(url.IndexOf("index/") + 6).Replace(",Indietro", ""));
                numeroPagina--;
                indexModel = new IndexViewModel(numeroPagina);
            }
            else
            {
                indexModel = new IndexViewModel(0);
            }
            if (url.Contains("?testoRicerca="))
            {
                parametroRicerca = url.Substring(url.IndexOf("?testoRicerca=") + 14);
                prodotti = parametroRicerca.Count() >= 3 ? operazioniProdottoMongo.VisualizzaProdotti(_mongoContext, parametroRicerca,numeroPagina) : operazioniProdottoMongo.VisualizzaProdotti(_mongoContext,numeroPagina);
            }
            else
            {
                prodotti = operazioniProdottoMongo.VisualizzaProdotti(_mongoContext,numeroPagina);
            }
            indexModel.InviaProdotti(prodotti);
            return View(indexModel);
        }
        public IActionResult VisualizzaUtenti()
        {
            VisualizzaUtentiViewModel visualizzaUtentiViewModel = new VisualizzaUtentiViewModel();
            OperazioniUsers operazioniUsers = new OperazioniUsers();

            string url = HttpContext.Request.GetDisplayUrl();
            string parametroRicerca="";
            List<User> users= new List<User>();

            
            if (url.Contains("?testoRicerca="))
            {
                parametroRicerca = url.Substring(url.IndexOf("?testoRicerca=") + 14);
                users= parametroRicerca.Count() >= 3 ? operazioniUsers.VisualizzaUsers(_context, parametroRicerca):new List<User>();
            }
            visualizzaUtentiViewModel.ListUsers=users;
            
            return View(visualizzaUtentiViewModel);
        }
        public async Task<IActionResult> ModificaRuolo(string id, List<string> ListRoles)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(id);

            await _userManager.RemoveFromRoleAsync(user, "Admin");
            await _userManager.RemoveFromRoleAsync(user, "User");

            if (ListRoles.Contains("User") && ListRoles.Contains("Admin"))
            {
                await _userManager.AddToRolesAsync(user, ListRoles);
            }
            else if(ListRoles.Contains("Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
            return await Task.FromResult(RedirectToAction("VisualizzaUtenti", "Home"));
        }
        public async Task<IActionResult> EliminaUtente(string id)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(id);

            await _userManager.DeleteAsync(user);

            return await Task.FromResult(RedirectToAction("VisualizzaUtenti", "Home"));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}