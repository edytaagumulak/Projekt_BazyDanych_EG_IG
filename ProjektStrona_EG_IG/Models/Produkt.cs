using System.ComponentModel.DataAnnotations;

namespace ProjektStrona_EG_IG.Models
{
    public class Produkt
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nazwa { get; set; }

        [Required]
        [MaxLength(100)]
        public string Opis { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Cena nie może być mniejsza niż 0.")]
        public decimal Cena { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Ilość dostępnych produktów nie może być mniejsza niż 0.")]
        public int IloscDostepna { get; set; }
    }
}
