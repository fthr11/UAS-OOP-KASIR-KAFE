using Microsoft.AspNetCore.Mvc;
using kasirkafe.Filters; // ← pastikan namespace filter di-import
using kasirkafe.Models;
using kasirkafe.Models.Interfaces;

namespace kasir_kafe.Controllers
{   
[AdminOnly] // Filter: hanya Admin yang bisa akses
public class AdminController : Controller
{
    private readonly IRepository<Product> _productRepo;

    public AdminController(IRepository<Product> productRepo)
    {
        _productRepo = productRepo;       // Dependency Injection
    }

    public IActionResult Index()
    {   
        return View();                     // Halaman dashboard admin
    }

    public IActionResult ManageProduct()
    {
        var products = _productRepo.GetAll(); // Ambil semua produk
        return View(products);
    }

    public IActionResult CreateProduct()
    {
        return View();                      // Form tambah produk
    }

    [HttpPost]
    public IActionResult CreateProduct(Product product)
    {
        if (!ModelState.IsValid)
            return View(product);

        _productRepo.Add(product);          // Tambah produk
        _productRepo.Save();                // SIMPAN KE DB

        return RedirectToAction("ManageProduct");
    }

    public IActionResult EditProduct(int id)
    {
        var product = _productRepo.GetById(id);  // Ambil produk by ID
        if (product == null) return NotFound();  // Cek error

        return View(product);                   // Tampilkan edit data
    }

    [HttpPost]
    public IActionResult EditProduct(Product product)
    {
        if (!ModelState.IsValid)
            return View(product);

        _productRepo.Update(product);       // Update produk
        _productRepo.Save();                // SIMPAN KE DB

        return RedirectToAction("ManageProduct");
    }

    public IActionResult DeleteProduct(int id)
    {
        _productRepo.Delete(id);            // Hapus produk
        _productRepo.Save();                // SIMPAN KE DB

        return RedirectToAction("ManageProduct");
    }
}

}
