using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sige_Erp.Models;
using Sige_Erp.Uteis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sige_Erp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Menu()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login(int? id)
        {
            //para realizar o logout
            if (id != null)
            {
                if (id == 0)
                {
                    HttpContext.Session.SetString("IdUsuarioLogado", string.Empty);
                    HttpContext.Session.SetString("NomeUsuarioLogado", string.Empty);
                }
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                bool loginOK = login.ValidarLogin();
                if (loginOK)
                {
                    HttpContext.Session.SetString("NrSeqUsuarioLogado", JsonConvert.SerializeObject(login.NrSeqUsuario));
                    HttpContext.Session.SetString("IdUsuarioLogado", login.NrSeqUsuario.ToString());
                    HttpContext.Session.SetString("NomeUsuarioLogado", login.NomeDeUsuario);
                    return RedirectToAction("Index", "Menu");
                }
                else
                {
                    if (TempData.ContainsKey("ErrorLogin"))
                    {
                        TempData.Remove("ErrorLogin");
                    }
                    TempData["ErrorLogin"] = "E-mail ou Senha são invalidos!";
                }
            }

            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CadastroPessoa()
        {
            return View("Pessoa/Manutencao"); 
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
