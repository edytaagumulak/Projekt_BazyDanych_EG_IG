using ProjektStrona_EG_IG.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjektStrona_EG_IG.Models;
public class DaneUzytkownika
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(25)]
    public string Imie { get; set; }

    [Required]
    [MaxLength(25)]
    public string Nazwisko { get; set; }

    [Required]
    [MaxLength(100)]
    public string Adres { get; set; }

    [Required]
    [MaxLength(6)]
    public string KodPocztowy { get; set; }

    [ForeignKey("Uzytkownik")]
    public int UzytkownikId { get; set; }
    public Uzytkownik Uzytkownik { get; set; }
}
