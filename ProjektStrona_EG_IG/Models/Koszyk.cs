using ProjektStrona_EG_IG.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjektStrona_EG_IG.Models
{
    public class Koszyk
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Uzytkownik")]
        public int UzytkownikId { get; set; }
        public Uzytkownik Uzytkownik { get; set; }
        [ForeignKey("Produkt")]
        public int ProduktId { get; set; }
        public Produkt Produkt { get; set; }
        [Required]
        public int Ilosc { get; set; } = 1;
    }
}

