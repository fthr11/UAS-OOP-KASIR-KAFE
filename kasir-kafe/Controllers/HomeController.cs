using Microsoft.AspNetCore.Mvc;
using kasirkafe.Data;
using kasirkafe.Models.ViewModels;
using kasirkafe.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using kasirkafe.Interfaces;

namespace kasir_kafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly CafeDbContext _context;
        private readonly ITransactionService _transactionService;

        public HomeController(CafeDbContext context, ITransactionService transactionService)
        {
            _context = context;
            _transactionService = transactionService;
        }

        // ===============================
        // HALAMAN KASIR
        // ===============================
        public async Task<IActionResult> Index()
        {
            var vm = new TransactionViewModel
            {
                AvailableProducts = await _context.Products.ToListAsync(),
                CartItems = GetCart(),
                PaymentAmount = TempData["PaymentAmount"] != null
                    ? decimal.Parse(TempData["PaymentAmount"]!.ToString()!)
                    : 0
            };
            return View(vm);
        }

        // ===============================
        // TAMBAH ITEM
        // ===============================
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var cart = GetCart();
            var product = _context.Products.Find(productId);
            if (product == null)
                return RedirectToAction("Index");

            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = 1,
                    Subtotal = product.Price
                });
            }
            else
            {
                item.Quantity++;
                item.Subtotal = item.Quantity * item.Price;
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // ===============================
        // KURANG / HAPUS ITEM
        // ===============================
        [HttpPost]
        public IActionResult ReduceFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                item.Quantity--;
                item.Subtotal = item.Quantity * item.Price;
                if (item.Quantity <= 0)
                    cart.Remove(item);
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // ===============================
        // KONFIRMASI PEMBAYARAN - FIXED VERSION
        // ===============================
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(decimal paymentAmount, string? customerName)
        {
            var cart = GetCart();

            // Validasi cart tidak kosong
            if (!cart.Any())
            {
                TempData["Error"] = "Keranjang kosong!";
                return RedirectToAction("Index");
            }

            // Hitung total
            decimal totalAmount = cart.Sum(c => c.Subtotal);

            // Validasi pembayaran cukup
            if (paymentAmount < totalAmount)
            {
                TempData["Error"] = "Uang yang dibayar kurang!";
                TempData["PaymentAmount"] = paymentAmount.ToString();
                return RedirectToAction("Index");
            }

            try
            {
                // Buat transaction code unik
                string transactionCode = $"TRX-{DateTime.Now:yyyyMMddHHmmss}";

                // Ambil user - PENTING: AsNoTracking untuk avoid tracking conflict
                var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync();
                if (user == null)
                {
                    TempData["Error"] = "User tidak ditemukan! Pastikan ada data user di database.";
                    return RedirectToAction("Index");
                }

                // Buat objek Transaction - TANPA navigation property dulu
                var transaction = new Transaction
                {
                    TransactionCode = transactionCode,
                    UserId = user.UserId,
                    CustomerName = customerName,
                    TotalAmount = totalAmount,
                    PaymentAmount = paymentAmount,
                    ChangeAmount = paymentAmount - totalAmount,
                    TransactionDate = DateTime.Now
                };

                // Set User navigation property dengan cara yang benar
                // Attach user yang sudah ada agar tidak di-insert ulang
                _context.Attach(user);
                transaction.User = user;

                // Buat list TransactionDetails
                var transactionDetails = new List<TransactionDetail>();

                foreach (var cartItem in cart)
                {
                    // Ambil product dengan AsNoTracking
                    var product = await _context.Products
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.ProductId == cartItem.ProductId);

                    if (product == null) continue;

                    // Attach product agar tidak di-insert ulang
                    _context.Attach(product);

                    var detail = new TransactionDetail
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Price,
                        Subtotal = cartItem.Subtotal,
                        Product = product,
                        Transaction = transaction
                    };

                    transactionDetails.Add(detail);
                }

                // Set TransactionDetails ke Transaction
                transaction.TransactionDetails = transactionDetails;

                // Simpan menggunakan TransactionService
                await _transactionService.CreateTransactionAsync(transaction);

                // Kosongkan cart setelah berhasil
                HttpContext.Session.Remove("Cart");

                TempData["Success"] = $"Transaksi {transactionCode} berhasil! Kembalian: Rp {transaction.ChangeAmount:N0}";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException dbEx)
            {
                // Error khusus database
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                TempData["Error"] = $"Database error: {innerMessage}";
                TempData["PaymentAmount"] = paymentAmount.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Error umum
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                TempData["Error"] = $"Gagal menyimpan transaksi: {innerMessage}";
                TempData["PaymentAmount"] = paymentAmount.ToString();
                return RedirectToAction("Index");
            }
        }

        // ===============================
        // SESSION CART
        // ===============================
        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(json)!;
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }
    }
}