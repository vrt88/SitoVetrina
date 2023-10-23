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
using SitoVetrina.Models.DbModels;
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
        private readonly IProdottoRepository _prodottoRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DapperContext _context;
        public HomeController(ILogger<HomeController> logger,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,IProdottoRepository prodottoRepository,DapperContext context)
        {
            _logger = logger;
            _userManager= userManager;
            _prodottoRepository = prodottoRepository;
            _roleManager= roleManager;
            _context=context;
        }
        public IActionResult Index(string testoRicerca,string pagina)
        {
            try
            {
                IndexViewModel indexModel;
                List<Prodotto> prodotti;
                string url = HttpContext.Request.GetDisplayUrl();
                int numeroPagina = 0;
                if (url.Contains("Avanti"))
                {
                    numeroPagina = Convert.ToInt16(pagina.Replace(",Avanti", ""));
                    numeroPagina++;
                    indexModel = new IndexViewModel(numeroPagina);
                }
                else if (url.Contains("Indietro"))
                {
                    numeroPagina = Convert.ToInt16(pagina.Replace(",Indietro", ""));
                    numeroPagina--;
                    indexModel = new IndexViewModel(numeroPagina);
                }
                else
                {
                    indexModel = new IndexViewModel(0);
                }
                if (testoRicerca != null)
                {
                    prodotti = testoRicerca.Count() >= 3 ? _prodottoRepository.VisualizzaProdotti(testoRicerca, numeroPagina) : _prodottoRepository.VisualizzaProdotti(numeroPagina);
                }
                else
                {
                    prodotti = _prodottoRepository.VisualizzaProdotti(numeroPagina);
                }
                indexModel.InviaProdotti(prodotti);
                return View(indexModel); 
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { exception = ex.Message });
            }
            
        }
        public IActionResult VisualizzaUtenti(string testoRicerca)
        {
            try
            {
                VisualizzaUtentiViewModel visualizzaUtentiViewModel = new VisualizzaUtentiViewModel(_roleManager.Roles.ToList());
                OperazioniUsers operazioniUsers = new OperazioniUsers();

                List<User> users = new List<User>();

                if (testoRicerca != null)
                {
                    users = testoRicerca.Count() >= 3 ? operazioniUsers.VisualizzaUsers(_context, testoRicerca) : new List<User>();
                }
                visualizzaUtentiViewModel.ListUsers = users;

                return View(visualizzaUtentiViewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { exception = ex.Message });
            }
        }
        public async Task<IActionResult> ModificaRuolo(string id, List<string> ListSelectedRoles)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                IList<IdentityRole> roles = _roleManager.Roles.ToList();

                for (int i = 0; i < ListSelectedRoles.Count; i++)
                {
                    if (ListSelectedRoles[i] != "false")
                    {
                        await _userManager.AddToRoleAsync(user, ListSelectedRoles[i]);
                    }
                    else
                    {
                        await _userManager.RemoveFromRoleAsync(user, roles[i].Name);
                    }
                }
                return await Task.FromResult(RedirectToAction("VisualizzaUtenti", "Home"));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(RedirectToAction("Error", "Home", new { exception = ex.Message }));
            }   
        }
        public async Task<IActionResult> EliminaUtente(string id)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);

                _prodottoRepository.EliminaCarrello(id);
                await _userManager.DeleteAsync(user);

                return await Task.FromResult(RedirectToAction("VisualizzaUtenti", "Home"));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(RedirectToAction("Error", "Home", new { exception = ex.Message }));
            }     
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string exception)
        {
            if(exception=="") 
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            else
            {
                return View(new ErrorViewModel { RequestId = exception ?? HttpContext.TraceIdentifier });
            }            
        }
    }
}