using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjektStrona_EG_IG.Areas.Identity.Data;

namespace ProjektStrona_EG_IG.Models
{
    public class Uzytkownik
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string Haslo { get; set; }

        [Required]
        [MaxLength(25)]
        public string Imie { get; set; } = "Wprowadź imię";

        [Required]
        [MaxLength(25)]
        public string Nazwisko { get; set; } = "Wprowadź nazwisko";

        [Required]
        [MaxLength(100)]
        public string Adres { get; set; } = "Wprowadź adres";

        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Kod pocztowy musi być w formacie XX-XXX.")]
        public string KodPocztowy { get; set; } = "00-000";

        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Pole musi zawierać dokładnie 9 cyfr.")]
        public string Telefon { get; set; } = "000000000";

        // Klucz obcy do AppUser
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}