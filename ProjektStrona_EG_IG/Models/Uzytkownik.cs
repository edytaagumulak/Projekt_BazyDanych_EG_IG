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

        // Klucz obcy do AppUser
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
