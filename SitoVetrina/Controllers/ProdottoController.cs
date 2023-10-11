using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SitoVetrina.Areas.Identity.Data;
using SitoVetrina.Context;
using SitoVetrina.Contracts;
using SitoVetrina.Models;
using SitoVetrina.Models.DbModels;
using SitoVetrina.Models.Operazioni;
using SitoVetrina.Models.ProdottoRepository;
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
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IProdottoRepository _prodottoRepository;
        public ProdottoController(IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, IProdottoRepository prodottoRepository)
        {
            this.hostEnvironment = hostEnvironment;       
            this.UserManager= userManager;
            this._prodottoRepository = prodottoRepository;
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
            OperazioniImmagine operazioniImmagine = new OperazioniImmagine();
            string nomeImmagine = operazioniImmagine.CreaImmagine(hostEnvironment, input.Immagine);
            string codiceProdotto= _prodottoRepository.CreaProdotto( input.NomeProdotto.Replace('\'', '"'), input.Descrizione.Replace('\'', '"'), input.Prezzo, nomeImmagine);
            return await Task.FromResult(RedirectToAction("DettagliProdotto","Prodotto",new { id=codiceProdotto }));
        }
        [HttpGet]
        public IActionResult DettagliProdotto(string id)
        {
            DettagliProdottoViewModel dettagliProdottoViewModel = new DettagliProdottoViewModel();
            dettagliProdottoViewModel.CodiceProdotto =id;
            dettagliProdottoViewModel.prodotto = _prodottoRepository.DettagliProdotto( dettagliProdottoViewModel.CodiceProdotto);
            return View(dettagliProdottoViewModel);
        }
        public async Task<IActionResult> Modifica(InputModel input,string id)
        {
            OperazioniImmagine operazioniImmagine = new OperazioniImmagine();
            string CodiceProdotto = id;
            Prodotto prodottoVecchio = _prodottoRepository.DettagliProdotto(CodiceProdotto);
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
            _prodottoRepository.ModificaProdotto( CodiceProdotto, input.NomeProdotto.Replace('\'', '"'), DescrizioneNuova, input.Prezzo, immagineNuova);
            return await Task.FromResult(RedirectToAction("DettagliProdotto", "Prodotto", new { id = CodiceProdotto }));
        }
        public async Task<IActionResult> Elimina(string id)
        {
            OperazioniImmagine operazioniImmagine = new OperazioniImmagine();
            string CodiceProdotto = id;
            Prodotto prodottoVecchio = _prodottoRepository.DettagliProdotto( CodiceProdotto);
            string immagineVecchia = prodottoVecchio.Immagine;
            operazioniImmagine.EliminaImmagine(immagineVecchia);
            _prodottoRepository.EliminaProdotto( CodiceProdotto);
            _prodottoRepository.EliminaProdottoCarrello(UserManager.GetUserId(User), CodiceProdotto);  
            return await Task.FromResult(RedirectToAction("Index","Home"));
        }
        [HttpPost]
        public async Task<IActionResult> AggiungiProdotto(string id)
        {      
            DettagliProdottoViewModel dettagliProdottoViewModel = new DettagliProdottoViewModel();
            dettagliProdottoViewModel.CodiceProdotto = id;
            dettagliProdottoViewModel.prodotto = _prodottoRepository.DettagliProdotto( dettagliProdottoViewModel.CodiceProdotto);
            dettagliProdottoViewModel.alert = _prodottoRepository.AggiungiProdottoCarrello(UserManager.GetUserId(User), dettagliProdottoViewModel.CodiceProdotto);
            return await Task.FromResult(View("DettagliProdotto", dettagliProdottoViewModel));
        }
        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult VisualizzaCarrello()
        {
            VisualizzaCarrelloViewModel visualizzaCarrelloViewModel = new VisualizzaCarrelloViewModel();
            visualizzaCarrelloViewModel.InviaProdotti(_prodottoRepository.VisualizzaProdottiCarrello(UserManager.GetUserId(User)));
            return View(visualizzaCarrelloViewModel);
        }
        public async Task<IActionResult> RimuoviProdottoCarrello(string id)
        {
            string codiceProdotto = id;
            _prodottoRepository.EliminaProdottoCarrello(UserManager.GetUserId(User), codiceProdotto);
            return await Task.FromResult(RedirectToAction("VisualizzaCarrello", "Prodotto"));
        }
        public async Task<IActionResult> CompraProdottoCarrello(string id)
        {
            string codiceProdotto = id;
            _prodottoRepository.EliminaProdottoCarrello(UserManager.GetUserId(User), codiceProdotto);
            return await Task.FromResult(RedirectToAction("VisualizzaCarrello", "Prodotto"));
        }
        public async Task<IActionResult> CompraProdottiCarrello()
        {
            _prodottoRepository.CompraProdottiCarrello(UserManager.GetUserId(User));
            return await Task.FromResult(RedirectToAction("VisualizzaCarrello", "Prodotto"));
        }
        public async Task<IActionResult> AggiornaProdottoCarrello(VisualizzaCarrelloViewModel visualizzaCarrelloViewModel, string id)
        { 
            string codiceProdotto = id;
            _prodottoRepository.AggiornaQuantitàProdotto(UserManager.GetUserId(User),codiceProdotto,Convert.ToInt16(visualizzaCarrelloViewModel.Quantità));
            return await Task.FromResult(RedirectToAction("VisualizzaCarrello", "Prodotto"));
        }
    }
}
