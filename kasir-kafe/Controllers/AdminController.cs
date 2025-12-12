using Microsoft.AspNetCore.Mvc;
using kasirkafe.Filters;
using kasirkafe.Models;
using kasirkafe.Models.Interfaces;

namespace kasir_kafe.Controllers
{   
    [AdminOnly]
    public class AdminController : Controller
    {
        private readonly IRepository<Product> _productRepo; // Repository untuk Product
        private readonly IWebHostEnvironment _webHost; // Untuk akses folder wwwroot
    
        public AdminController(IRepository<Product> productRepo, IWebHostEnvironment webHost)
        {
            _productRepo = productRepo;
            _webHost = webHost;
        }

        public IActionResult Index()
        {   
            return View();
        }

        public IActionResult ManageProduct()
        {
            var products = _productRepo.GetAll();
            return View(products);
        }

        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            // Upload gambar
            if (product.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images/products");
                Directory.CreateDirectory(uploadsFolder); // Buat folder jika belum ada
                
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream);
                }
                
                product.Image = "/images/products/" + uniqueFileName;
            }

            _productRepo.Add(product);
            _productRepo.Save();
            return RedirectToAction("ManageProduct");
        }

        public IActionResult EditProduct(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            // Ambil data lama dari database 
            var oldProduct = _productRepo.GetById(product.ProductId);
            if (oldProduct == null)
                return NotFound();

            // Update field biasa
            oldProduct.ProductName = product.ProductName;
            oldProduct.Price = product.Price;
            oldProduct.Category = product.Category;

            // Jika gambar baru diupload
            if (product.ImageFile != null)
            {
                // Hapus gambar lama
                if (!string.IsNullOrEmpty(oldProduct.Image))
                {
                    string oldImagePath = Path.Combine(_webHost.WebRootPath, oldProduct.Image.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Upload gambar baru
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images/products");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream);
                }

                oldProduct.Image = "/images/products/" + uniqueFileName;
            }

            // Simpan perubahan 
            _productRepo.Save();

            return RedirectToAction("ManageProduct");
        }

        public IActionResult DeleteProduct(int id)
        {
            // Hapus gambar dari folder
            var product = _productRepo.GetById(id);
            if (product != null && !string.IsNullOrEmpty(product.Image))
            {
                string imagePath = Path.Combine(_webHost.WebRootPath, product.Image.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _productRepo.Delete(id);
            _productRepo.Save();
            return RedirectToAction("ManageProduct");
        }
    }
}