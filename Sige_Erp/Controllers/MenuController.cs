using Microsoft.AspNetCore.Mvc;

namespace Sige_Erp.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
