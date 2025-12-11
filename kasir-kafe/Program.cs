<<<<<<< Updated upstream
=======

using kasirkafe.Data;
using kasirkafe.Models.Interfaces;
using kasirkafe.Models.Repositories; 
using kasirkafe.Models; 
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
>>>>>>> Stashed changes
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
<<<<<<< Updated upstream
=======
builder.Services.AddSession();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
>>>>>>> Stashed changes

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
