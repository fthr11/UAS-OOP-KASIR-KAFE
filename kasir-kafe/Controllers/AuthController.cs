using Microsoft.AspNetCore.Mvc;

namespace kasir_kafe.Controllers;

public class AuthController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        // ini akan me-render Views/Auth/Login.cshtml
        return View();
    }

    [HttpPost]
    public IActionResult Login(string Email, string Password)
    {
        // TODO: nanti isi logika cek user di sini
        // kalau gagal: return View();
        // kalau sukses:
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Register(string Name, string Email, string Password, string ConfirmPassword)
    {
        // TODO: simpan user ke database
        // sementara redirect aja

        return RedirectToAction("Login");
    }

}
