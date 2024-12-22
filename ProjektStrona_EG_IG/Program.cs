using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektStrona_EG_IG.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Connection string do bazy danych
var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Konfiguracja us³ug Identity
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Dodanie obs³ugi ról
    .AddEntityFrameworkStores<AppDbContext>();

// Dodanie us³ug MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Inicjalizacja ról i u¿ytkowników
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services); // Wywo³anie metody inicjalizacyjnej
}

// Middleware dla aplikacji
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Produkt/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Produkt}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
