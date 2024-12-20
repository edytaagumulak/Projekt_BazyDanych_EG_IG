using System.ComponentModel.DataAnnotations;

namespace ProjektStrona_EG_IG.Models
{
    public class Produkt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nazwa { get; set; }

        public string Opis { get; set; }

        [Required]
        public decimal Cena { get; set; }

        [Required]
        public int IloscDostepna { get; set; }

    }
}