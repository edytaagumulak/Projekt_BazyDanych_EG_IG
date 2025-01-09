using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [NotMapped]
        public string ZdjecieUrl => Produkt?.ZdjecieUrl ?? "/images/default.png"; // Domyślne zdjęcie, jeśli brak
       
        [Required]
        public int Ilosc { get; set; } = 1;
    }
}