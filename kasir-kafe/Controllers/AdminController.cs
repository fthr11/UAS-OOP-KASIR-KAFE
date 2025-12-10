using Microsoft.AspNetCore.Mvc;

namespace kasir_kafe.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}