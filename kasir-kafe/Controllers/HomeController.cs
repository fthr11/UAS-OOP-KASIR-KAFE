using Microsoft.AspNetCore.Mvc;
using kasirkafe.Data;
using kasirkafe.Models.ViewModels;
using kasirkafe.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using kasirkafe.Models.Interfaces;

namespace kasir_kafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly CafeDbContext _context;


        public HomeController(CafeDbContext context)
        {
            _context = context;
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
        // KONFIRMASI PEMBAYARAN
        // ===============================
        [HttpPost]
        public IActionResult ConfirmPayment(decimal paymentAmount)
        {
            TempData["PaymentAmount"] = paymentAmount.ToString();
            return RedirectToAction("Index");
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
