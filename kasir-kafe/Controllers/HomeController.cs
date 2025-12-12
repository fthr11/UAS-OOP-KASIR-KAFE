using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kasirkafe.ViewModels;
using kasirkafe.Data;
using kasirkafe.Models;
using Microsoft.EntityFrameworkCore;

namespace kasir_kafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CafeDbContext _context;

        public HomeController(ILogger<HomeController> logger, CafeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // === HALAMAN HOME / KASIR ===
        public async Task<IActionResult> Index()
        {
            // Ambil semua produk aktif dengan stock > 0
            var products = await _context.Products.ToListAsync();


            return View(products); // kirim ke Index.cshtml
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
