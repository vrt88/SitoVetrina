using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SitoVetrina.Areas.Identity.Data;
using SitoVetrina.Context;
using SitoVetrina.Contracts;
using SitoVetrina.Models;
using SitoVetrina.Models.Operazioni;
using SitoVetrina.Models.ProdottoViewModels;
using System;
using System.Data;
using System.Security.Claims;
using System.Security.Policy;
using System.Xml.Linq;

namespace SitoVetrina.Controllers
{
    public class ProdottoController : Controller
    {
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly DapperContext context;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly MongoDBContext MongoContext;
        public ProdottoController(IWebHostEnvironment hostEnvironment, DapperContext context, UserManager<ApplicationUser> userManager,MongoDBContext mongoContext)
        {
            this.hostEnvironment = hostEnvironment;
            this.context = context;          
            this.UserManager= userManager;
            this.MongoContext= mongoContext;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreaProdotto()
        {
            CreaProdottoViewModel creaProdottoViewModel = new CreaProdottoViewModel();
            return View(creaProdottoViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreaProdotto(InputModel input)
        {
            OperazioniProdottoMongo operazioniProdotto = new OperazioniProdottoMongo();
            OperazioniImmagine operazioniImmagine = new OperazioniImmagine();
            string nomeImmagine = operazioniImmagine.CreaImmagine(hostEnvironment, input.Immagine);
            string codiceProdotto= operazioniProdotto.CreaProdotto(MongoContext, input.NomeProdotto.Replace('\'', '"'), input.Descrizione.Replace('\'', '"'), input.Prezzo, nomeImmagine);
            return await Task.FromResult(RedirectToAction("DettagliProdotto","Prodotto",new { _id=codiceProdotto }));
        }
        [HttpGet]
        public IActionResult DettagliProdotto()
        {
            OperazioniProdottoMongo operazioniProdotto = new OperazioniProdottoMongo();
            string url = HttpContext.Request.GetDisplayUrl();
            DettagliProdottoViewModel dettagliProdottoViewModel = new DettagliProdottoViewModel();
            dettagliProdottoViewModel.CodiceProdotto = url.Contains("_id=") ? url.Substring(url.IndexOf("_id=") + 4) : url.Substring(url.IndexOf("DettagliProdotto/") + 17);
            dettagliProdottoViewModel.prodotto = operazioniProdotto.DettagliProdotto(MongoContext, dettagliProdottoViewModel.CodiceProdotto);
            return View(dettagliProdottoViewModel);
        }
        public async Task<IActionResult> Modifica(InputModel input)
        {
            OperazioniProdottoMongo operazioniProdotto = new OperazioniProdottoMongo();
            OperazioniImmagine operazioniImmagine = new OperazioniImmagine();
            string url = HttpContext.Request.GetDisplayUrl();
            string CodiceProdotto = url.Substring(url.IndexOf("Modifica/") + 9);
            ProdottoMongo prodottoVecchio = operazioniProdotto.DettagliProdotto(MongoContext,CodiceProdotto);
            string immagineVecchia = prodottoVecchio.Immagine;
            string DescrizioneVecchia = prodottoVecchio.Descrizione;
            string immagineNuova = "";
            if (input.Immagine != null)
            {
                operazioniImmagine.EliminaImmagine(immagineVecchia);
                immagineNuova = operazioniImmagine.CreaImmagine(hostEnvironment, input.Immagine);
            }
            else
            {
                immagineNuova = immagineVecchia;
            }
            string DescrizioneNuova = "";
            DescrizioneNuova = input.Descrizione != null ? input.Descrizione.Replace('\'', '"') : DescrizioneVecchia;
            operazioniProdotto.ModificaProdotto(MongoContext, CodiceProdotto, input.NomeProdotto.Replace('\'', '"'), DescrizioneNuova, input.Prezzo, immagineNuova);
            return await Task.FromResult(RedirectToAction("DettagliProdotto", "Prodotto", new { _id = CodiceProdotto }));
        }
        public async Task<IActionResult> Elimina()
        {
            OperazioniProdottoMongo operazioniProdotto = new OperazioniProdottoMongo();
            OperazioniCarrelloMongo operazioniCarrelloMongo= new OperazioniCarrelloMongo();
            OperazioniImmagine operazioniImmagine = new OperazioniImmagine();
            string url = HttpContext.Request.GetDisplayUrl();
            string CodiceProdotto = url.Substring(url.IndexOf("Elimina/") + 8);
            ProdottoMongo prodottoVecchio = operazioniProdotto.DettagliProdotto(MongoContext, CodiceProdotto);
            string immagineVecchia = prodottoVecchio.Immagine;
            operazioniImmagine.EliminaImmagine(immagineVecchia);
            operazioniProdotto.EliminaProdotto(MongoContext, CodiceProdotto);
            operazioniCarrelloMongo.EliminaProdottoCarrello(MongoContext, UserManager.GetUserId(User).Replace("-", ""), CodiceProdotto);  
            return await Task.FromResult(RedirectToAction("Index","Home"));
        }
        [HttpPost]
        public async Task<IActionResult> AggiungiProdotto()
        {      
            OperazioniProdottoMongo operazioniProdotto = new OperazioniProdottoMongo();
            DettagliProdottoViewModel dettagliProdottoViewModel = new DettagliProdottoViewModel();
            OperazioniCarrelloMongo operazioniCarrello = new OperazioniCarrelloMongo();
            string url = HttpContext.Request.GetDisplayUrl();
            dettagliProdottoViewModel.CodiceProdotto = url.Substring(url.IndexOf("AggiungiProdotto/") + 17);
            dettagliProdottoViewModel.prodotto = operazioniProdotto.DettagliProdotto(MongoContext, dettagliProdottoViewModel.CodiceProdotto);
            dettagliProdottoViewModel.alert = operazioniCarrello.AggiungiProdottoCarrello(MongoContext, UserManager.GetUserId(User), dettagliProdottoViewModel.CodiceProdotto);
            return await Task.FromResult(View("DettagliProdotto", dettagliProdottoViewModel));
        }
        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult VisualizzaCarrello()
        {
            OperazioniCarrelloMongo operazioniCarrello = new OperazioniCarrelloMongo();
            VisualizzaCarrelloViewModel visualizzaCarrelloViewModel = new VisualizzaCarrelloViewModel();
            visualizzaCarrelloViewModel.InviaProdotti(operazioniCarrello.VisualizzaProdottiCarrello(MongoContext, UserManager.GetUserId(User).Replace("-", "")));
            return View(visualizzaCarrelloViewModel);
        }
        public async Task<IActionResult> RimuoviProdottoCarrello()
        {
            OperazioniCarrelloMongo operazioniCarrello = new OperazioniCarrelloMongo();
            string url = HttpContext.Request.GetDisplayUrl();
            string codiceProdotto = url.Substring(url.IndexOf("RimuoviProdottoCarrello/") + 24);
            operazioniCarrello.EliminaProdottoCarrello(MongoContext, UserManager.GetUserId(User).Replace("-", ""), codiceProdotto);
            return await Task.FromResult(RedirectToAction("VisualizzaCarrello", "Prodotto"));
        }
        public async Task<IActionResult> CompraProdottoCarrello()
        {
            OperazioniCarrelloMongo operazioniCarrello = new OperazioniCarrelloMongo();
            string url = HttpContext.Request.GetDisplayUrl();
            string codiceProdotto = url.Substring(url.IndexOf("CompraProdottoCarrello/") + 23,24);
            operazioniCarrello.EliminaProdottoCarrello(MongoContext, UserManager.GetUserId(User).Replace("-", ""), codiceProdotto);
            return await Task.FromResult(RedirectToAction("VisualizzaCarrello", "Prodotto"));
        }
        public async Task<IActionResult> CompraProdottiCarrello()
        {
            OperazioniCarrelloMongo operazioniCarrello = new OperazioniCarrelloMongo();
            operazioniCarrello.CompraProdottiCarrello(MongoContext,UserManager.GetUserId(User).Replace("-",""));
            return await Task.FromResult(RedirectToAction("VisualizzaCarrello", "Prodotto"));
        }
        public async Task<IActionResult> AggiornaProdottoCarrello(VisualizzaCarrelloViewModel visualizzaCarrelloViewModel)
        {
            OperazioniCarrelloMongo operazioniCarrello = new OperazioniCarrelloMongo();
            string url = HttpContext.Request.GetDisplayUrl();
            string codiceProdotto = url.Substring(url.IndexOf("AggiornaProdottoCarrello/") + 25,24);
            operazioniCarrello.AggiornaQuantitàProdotto(MongoContext, UserManager.GetUserId(User),codiceProdotto,Convert.ToInt16(visualizzaCarrelloViewModel.Quantità));
            return await Task.FromResult(RedirectToAction("VisualizzaCarrello", "Prodotto"));
        }
    }
}
