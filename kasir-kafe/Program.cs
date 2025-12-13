using kasirkafe.Data;
using kasirkafe.Models.Interfaces;
using kasirkafe.Models.Repositories;
using kasirkafe.Models;
using kasirkafe.Interfaces;
using kasirkafe.Repositories;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

DbTest.Test();

builder.Services.AddDbContext<CafeDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 40)) 
    ));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Dependency Injection - URUTAN PENTING!
// 1. Repository Generic dulu
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// 2. TransactionService (menggunakan IRepository)
builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseStaticFiles();

// Session HARUS setelah UseRouting dan sebelum MapControllerRoute
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();