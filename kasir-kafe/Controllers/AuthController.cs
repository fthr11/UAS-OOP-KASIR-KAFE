using Microsoft.AspNetCore.Mvc;
using kasirkafe.Data;
using kasirkafe.Models;
using System.Linq;


namespace kasir_kafe.Controllers;

public class AuthController : Controller
{
    private readonly CafeDbContext _context;

        public AuthController(CafeDbContext context)
        {
            _context = context;
        }

    [HttpGet]
    public IActionResult Login()
    {
        // ini akan me-render Views/Auth/Login.cshtml
        return View();
    }

    [HttpPost]
    public IActionResult Login(string Email, string Password)
    {
            var user = _context.Users
            .FirstOrDefault(u => u.Email == Email && u.Password == Password);

        if (user == null)
        {
            ViewBag.Error = "Username atau password salah!";
            return View();
        }

        // Simpan session
        HttpContext.Session.SetString("UserId", user.UserId.ToString());
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("Role", user.Role);

        // Redirect berdasarkan role
        if (user.Role == "Admin")
            return RedirectToAction("Index", "Admin"); // halaman Admin
        else
            return RedirectToAction("Index", "Home");  // halaman Kasir/Home
        }

        [HttpGet]
        public IActionResult Register()
        {
        // ini akan me-render Views/Auth/Register.cshtml
        return View(); // otomatis mencari Register.cshtml sesuai nama method
    }

    [HttpPost]
    public IActionResult Register(string Name, string Username, string Email, string Password, string ConfirmPassword)
    {
        if (Password != ConfirmPassword)
        {
            ViewBag.Error = "Password dan Confirm Password tidak sama!";
            return View();
        }

        var newUser = new User
        {
            Username = Name,     // dari input form
            FullName = Name,    // atau ambil dari input FullName
            Email = Email,     
            Password = Password, // dari input form
            Role = "Kasir",      // default role
            CreatedAt = DateTime.Now
        };
    _context.Users.Add(newUser);
    _context.SaveChanges();

    return RedirectToAction("Login");
    }
}
