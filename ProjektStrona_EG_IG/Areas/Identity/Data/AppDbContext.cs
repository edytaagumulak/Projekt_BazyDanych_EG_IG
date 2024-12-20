using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjektStrona_EG_IG.Areas.Identity.Data;
using ProjektStrona_EG_IG.Models;

namespace ProjektStrona_EG_IG.Areas.Identity.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Uzytkownik> Uzytkownik { get; set; }
    public DbSet<DaneUzytkownika> DaneUzytkownik { get; set; }
    public DbSet<Koszyk> Koszyk { get; set; }

    public DbSet<Produkt> Produkt { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
