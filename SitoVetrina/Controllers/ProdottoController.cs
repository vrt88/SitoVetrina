using Amazon.Runtime.SharedInterfaces;
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
using System.Diagnostics;
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
            try
            {
                OperazioniImmagine operazioniImmagine = new OperazioniImmagine();
                string nomeImmagine;
                if (input.Immagine != null)
                {
                    nomeImmagine = operazioniImmagine.CreaImmagine(hostEnvironment, input.Immagine);
                }
                else
                {
                    return await Task.FromResult(RedirectToAction("Error", "Home", new { exception = "Immagine non inserita" }));
                }
                string codiceProdotto = _prodottoRepository.CreaProdotto(input.NomeProdotto.Replace('\'', '"'), input.Descrizione.Replace('\'', '"'), Convert.ToDecimal(input.Prezzo.Replace('.', ',')), nomeImmagine);
                return await Task.FromResult(RedirectToAction("DettagliProdotto", "Prodotto", new { id = codiceProdotto }));
            }
            catch (FormatException)
            {
                return await Task.FromResult(RedirectToAction("Error", "Home", new { exception = "Il prezzo non deve contenere caratteri"}));
            }
            catch(Exception)
            {
                return await Task.FromResult(RedirectToAction("Error", "Home", new { exception = "Errore generico, riprovare più tardi"}));
            }

        }
        [HttpGet]
        public IActionResult DettagliProdotto(string id)
        {
            try
            {
                DettagliProdottoViewModel dettagliProdottoViewModel = new DettagliProdottoViewModel();
                dettagliProdottoViewModel.CodiceProdotto = id;
                dettagliProdottoViewModel.prodotto = _prodottoRepository.DettagliProdotto(dettagliProdottoViewModel.CodiceProdotto);
                return View(dettagliProdottoViewModel);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { exception = "Errore generico, riprovare più tardi" });
            }
        }
        public async Task<IActionResult> Modifica(InputModel input,string id)
        {
            try
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
                _prodottoRepository.ModificaProdotto(CodiceProdotto, input.NomeProdotto.Replace('\'', '"'), DescrizioneNuova, Convert.ToDecimal(input.Prezzo.Replace('.', ',')), immagineNuova);
                return await Task.FromResult(RedirectToAction("DettagliProdotto", "Prodotto", new { id = CodiceProdotto }));
            }
            catch (FormatException)
            {
                return await Task.FromResult(RedirectToAction("Error", "Home", new { exception = "Il prezzo non deve contenere caratteri" }));
            }
            catch (Exception)
            {
                return await Task.FromResult(RedirectToAction("Error", "Home", new { exception = "Errore generico, riprovare più tardi" }));
            }
        }
        public async Task<IActionResult> Elimina(string id)
        {
            try
            {
                OperazioniImmagine operazioniImmagine = new OperazioniImmagine();
                string CodiceProdotto = id;
                Prodotto prodottoVecchio = _prodottoRepository.DettagliProdotto(CodiceProdotto);
                string immagineVecchia = prodottoVecchio.Immagine;
                operazioniImmagine.EliminaImmagine(immagineVecchia);
                _prodottoRepository.EliminaProdotto(CodiceProdotto);
                _prodottoRepository.EliminaProdottoCarrello(UserManager.GetUserId(User), CodiceProdotto);
                return await Task.FromResult(RedirectToAction("Index", "Home"));
            }
            catch (Exception)
            {
                return await Task.FromResult(RedirectToAction("Error", "Home", new { exception = "Errore generico, riprovare più tardi" }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> AggiungiProdotto(string id, Prodotto prodotto)
        {
            try
            {
                DettagliProdottoViewModel dettagliProdottoViewModel = new DettagliProdottoViewModel();
                dettagliProdottoViewModel.CodiceProdotto = id;
                dettagliProdottoViewModel.prodotto = _prodottoRepository.DettagliProdotto(dettagliProdottoViewModel.CodiceProdotto);
                dettagliProdottoViewModel.alert = "Prodotto aggiunto";
                _prodottoRepository.AggiungiProdottoCarrello(UserManager.GetUserId(User), dettagliProdottoViewModel.CodiceProdotto);
                return await Task.FromResult(View("DettagliProdotto", dettagliProdottoViewModel));
            }
            catch
            {
                DettagliProdottoViewModel dettagliProdottoViewModel = new DettagliProdottoViewModel();

                return await Task.FromResult(View("DettagliProdotto", dettagliProdottoViewModel));
            }
        }
        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult VisualizzaCarrello()
        {
            try
            {
                VisualizzaCarrelloViewModel visualizzaCarrelloViewModel = new VisualizzaCarrelloViewModel();
                visualizzaCarrelloViewModel.InviaProdotti(_prodottoRepository.VisualizzaProdottiCarrello(UserManager.GetUserId(User)));
                return View(visualizzaCarrelloViewModel);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { exception = "Errore generico, riprovare più tardi" });
            }
        }
        public async Task<IActionResult> RimuoviProdottoCarrello(string id)
        {
            try
            {
                string codiceProdotto = id;
                _prodottoRepository.EliminaProdottoCarrello(UserManager.GetUserId(User), codiceProdotto);
                return await Task.FromResult(RedirectToAction("VisualizzaCarrello", "Prodotto"));
            }
            catch (Exception)
            {
                return await Task.FromResult(RedirectToAction("Error", "Home", new { exception = "Errore generico, riprovare più tardi" }));
            }
        }
        public async Task<IActionResult> CompraProdottoCarrello(string id)
        {
            try
            {
                string codiceProdotto = id;
                _prodottoRepository.EliminaProdottoCarrello(UserManager.GetUserId(User), codiceProdotto);
                return await Task.FromResult(RedirectToAction("VisualizzaCarrello", "Prodotto"));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(RedirectToAction("Error", "Home", new { exception = ex.Message }));
            }
            
        }
        public async Task<IActionResult> CompraProdottiCarrello()
        {
            try
            {
                _prodottoRepository.EliminaCarrello(UserManager.GetUserId(User));
                return await Task.FromResult(RedirectToAction("VisualizzaCarrello", "Prodotto"));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(RedirectToAction("Error", "Home", new { exception = ex.Message }));
            }
            
        }
        public async Task<IActionResult> AggiornaProdottoCarrello(VisualizzaCarrelloViewModel visualizzaCarrelloViewModel, string id)
        {
            try
            {
                string codiceProdotto = id;
                _prodottoRepository.AggiornaQuantitàProdotto(UserManager.GetUserId(User), codiceProdotto, Convert.ToInt16(visualizzaCarrelloViewModel.Quantità));
                return await Task.FromResult(RedirectToAction("VisualizzaCarrello", "Prodotto"));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(RedirectToAction("Error", "Home", new { exception = ex.Message }));
            }
            
        }
    }
}
