using Microsoft.AspNetCore.Mvc;

namespace SistemaAutomotriz.Controllers
{
    public class ClienteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
