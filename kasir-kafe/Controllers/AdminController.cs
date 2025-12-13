using Microsoft.AspNetCore.Mvc; 
using kasirkafe.Filters;
using kasirkafe.Models; 
using kasirkafe.Models.Interfaces;
using kasirkafe.Interfaces;

namespace kasir_kafe.Controllers 
{
    [AdminOnly] // Hanya bisa diakses user dnegan roleadmin
    public class AdminController : Controller // Mendefinisikan kelas controller yang mewarisi dari Controller dari framework ASP core MVC
    {
        private readonly IRepository<Product> _productRepo; // field private untuk repository produk
        private readonly IWebHostEnvironment _webHost; // field private untuk mengakses wwwroot

        private readonly ITransactionService _transactionService;
        public AdminController(
            IRepository<Product> productRepo,
            IWebHostEnvironment webHost,
            ITransactionService transactionService)
        {
            _productRepo = productRepo;
            _webHost = webHost;
            _transactionService = transactionService;
        }

        // Halaman Admin (Action Method)
        public IActionResult Index()
        {
            return View();
        }

        // Menampilkan daftar produk (Action Method)
        public IActionResult ManageProduct()
        {
            var products = _productRepo.GetAll(); // GetAll digunakan untuk menampilkan semua data produk
            return View(products);
        }

        // Menampilkan form CreateProduct (Action Method - GET)
        public IActionResult CreateProduct(string? type = "Product") // Parameter type untuk menentukan jenis produk 
        {
            ViewBag.ProductType = type; // ViewBag digunakan untuk mengirim tipe produk ke View
            return View(); // Mengembalikan View (CreateProduct.cshtml)
        }

        [HttpPost] // Atribut untuk menangani request HTTP POST
        public async Task<IActionResult> CreateProduct(string productType, Product product, string? jenisMakanan, string? jenisMinuman) // Action method POST untuk membuat produk
        {
            if (!ModelState.IsValid) // Memeriksa apakah data model yang di-bind valid berdasarkan data model
                return View(product); // Jika tidak valid, kembalikan View dengan data yang tidak valid

            // Buat instance sesuai tipe
            Product newProduct; // Deklarasi variabel untuk produk baru

            // Validasi tipe produk dan buat instance yang sesuai
            if (productType == "FoodProduct")
            {
                newProduct = new FoodProduct
                {
                    ProductName = product.ProductName,
                    Category = product.Category,
                    Price = product.Price,
                    CreatedAt = DateTime.Now,
                    JenisMakanan = jenisMakanan // Mengatur properti spesifik FoodProduct
                };
            }
            else if (productType == "DrinkProduct")
            {
                newProduct = new DrinkProduct
                {
                    ProductName = product.ProductName,
                    Category = product.Category,
                    Price = product.Price,
                    CreatedAt = DateTime.Now,
                    JenisMinuman = jenisMinuman // Mengatur properti spesifik DrinkProduct
                };
            }
            else
            {
                newProduct = product; // Menggunakan instance Product yang telah dibuat
                newProduct.CreatedAt = DateTime.Now;
            }

            // Upload gambar
            if (product.ImageFile != null) // Memeriksa apakah ada file gambar yang diupload
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images/products"); // Menentukan path folder upload di wwwroot
                Directory.CreateDirectory(uploadsFolder); // kalau folder belum dibuat maka akan otomatis terbuat jika kita mengupload gambar

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName; // untuk membuat nama file yang berbeda dan auto generate
                string filePath = Path.Combine(uploadsFolder, uniqueFileName); // Menggabungkan path folder dan nama file

                using (var fileStream = new FileStream(filePath, FileMode.Create)) // Membuat FileStream untuk menulis file
                {
                    await product.ImageFile.CopyToAsync(fileStream); // Menyalin konten file yang diupload 
                }

                newProduct.Image = "/images/products/" + uniqueFileName; // Menyimpan path gambar local 
            }

            _productRepo.Add(newProduct); // Menambahkan produk baru ke repository
            _productRepo.Save(); // Menyimpan perubahan ke database
            return RedirectToAction("ManageProduct");
        }

        // Action method untuk menampilkan form edit produk
        public IActionResult EditProduct(int id)
        {
            var product = _productRepo.GetById(id); // Mengambil produk berdasarkan ID
            if (product == null) return NotFound(); // Jika produk tidak ditemukan, kembalikan NotFound (HTTP 404)

            // Pass tipe product ke view
            ViewBag.ProductType = product.GetType().Name; // Menyimpan nama tipe produk ke ViewBag
            return View(product); // Mengirim produk ke View (EditProduct.cshtml)
        }

        [HttpPost] // Atribut untuk menangani request HTTP POST
        public async Task<IActionResult> EditProduct(Product product, string? jenisMakanan, string? jenisMinuman)
        {
            if (!ModelState.IsValid) // Memeriksa validitas model
                return View(product); // Jika tidak valid, kembalikan View

            var oldProduct = _productRepo.GetById(product.ProductId); // Mengambil produk yang sudah ada dari repository
            if (oldProduct == null)
                return NotFound(); // Jika produk tidak ditemukan, kembalikan NotFound

            // untuk update properties umum
            oldProduct.ProductName = product.ProductName; // Memperbarui nama produk
            oldProduct.Price = product.Price; // Memperbarui harga
            oldProduct.Category = product.Category; // Memperbarui kategori

            // untuk mengpdate properties khusus 
            if (oldProduct is FoodProduct foodProduct) // Memeriksa apakah produk adalah FoodProduct
            {
                foodProduct.JenisMakanan = jenisMakanan; // Memperbarui properti spesifik FoodProduct
            }
            else if (oldProduct is DrinkProduct drinkProduct) // Memeriksa apakah produk adalah DrinkProduct
            {
                drinkProduct.JenisMinuman = jenisMinuman; // Memperbarui properti spesifik DrinkProduct
            }

            // Jika gambar baru diupload
            if (product.ImageFile != null) // Memeriksa apakah ada file gambar baru yang diupload
            {
                // Hapus gambar lama
                if (!string.IsNullOrEmpty(oldProduct.Image)) // Memeriksa apakah ada path gambar lama
                {
                    string oldImagePath = Path.Combine(_webHost.WebRootPath, oldProduct.Image.TrimStart('/')); // Mendapatkan path fisik gambar lama
                    if (System.IO.File.Exists(oldImagePath)) // Memeriksa apakah file gambar lama ada
                    {
                        System.IO.File.Delete(oldImagePath); // Menghapus file gambar lama
                    }
                }

                // Upload gambar baru (prosesnya sama seperti di CreateProduct)
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images/products"); // Menentukan path folder upload
                Directory.CreateDirectory(uploadsFolder); // Membuat folder jika belum ada

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName; // Membuat nama file unik
                string filePath = Path.Combine(uploadsFolder, uniqueFileName); // Menggabungkan path folder dan nama file

                using (var fileStream = new FileStream(filePath, FileMode.Create)) // Membuat FileStream
                {
                    await product.ImageFile.CopyToAsync(fileStream); // Menyalin file baru
                }

                oldProduct.Image = "/images/products/" + uniqueFileName; // Memperbarui path gambar relatif di model
            }

            _productRepo.Save(); // Menyimpan perubahan ke database
            return RedirectToAction("ManageProduct"); // Mengarahkan pengguna kembali
        }

        // Action method untuk menghapus produk
        public IActionResult DeleteProduct(int id)
        {
            var product = _productRepo.GetById(id); // Mengambil produk berdasarkan ID
            if (product != null && !string.IsNullOrEmpty(product.Image)) // Memeriksa jika produk ada dan memiliki gambar
            {
                string imagePath = Path.Combine(_webHost.WebRootPath, product.Image.TrimStart('/')); // Mendapatkan path fisik gambar
                if (System.IO.File.Exists(imagePath)) // Memeriksa apakah file gambar ada
                {
                    System.IO.File.Delete(imagePath); // Menghapus file gambar
                }
            }

            _productRepo.Delete(id); // Menghapus produk dari repository
            _productRepo.Save(); // Menyimpan perubahan ke database
            return RedirectToAction("ManageProduct"); // Mengarahkan pengguna kembali
        }
        
        // ===============================
        // TRANSAKSI
        // ===============================
        public async Task<IActionResult> ManageTransaction()
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return View(transactions);
        }

        // Menampilkan detail transaksi
        public async Task<IActionResult> TransactionDetail(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
                return NotFound();

            return View(transaction);
        }

        // Hapus transaksi
        [HttpPost]
        public IActionResult DeleteTransaction(int id)
        {
            // Gunakan repository untuk hapus
            var transactionRepo = HttpContext.RequestServices.GetService<IRepository<Transaction>>();
            
            if (transactionRepo != null)
            {
                transactionRepo.Delete(id);
                transactionRepo.Save();
                TempData["Success"] = "Transaksi berhasil dihapus!";
            }
            else
            {
                TempData["Error"] = "Gagal menghapus transaksi!";
            }

            return RedirectToAction("ManageTransaction");
        }
    }
}