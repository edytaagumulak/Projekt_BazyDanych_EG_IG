using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjektStrona_EG_IG.Models;

namespace ProjektStrona_EG_IG.Areas.Identity.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    //DbSet<T> umożliwia wykonywanie operacji CRUD na danych w tabelach odpowiadających encjom:
    public DbSet<Produkt> Produkt { get; set; }
    public DbSet<Uzytkownik> Uzytkownik { get; set; }
    public DbSet<Koszyk> Koszyk { get; set; }
    public DbSet<Zamowienie> Zamowienia { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //Tworzenie ról do podziału uprawnień
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "User", NormalizedName = "USER" }
        );

    }
}
