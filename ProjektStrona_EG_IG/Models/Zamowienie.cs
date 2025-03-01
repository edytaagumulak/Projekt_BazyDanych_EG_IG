﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektStrona_EG_IG.Models
{
    public class Zamowienie
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Uzytkownik")]
        public int UzytkownikId { get; set; }
        public Uzytkownik Uzytkownik { get; set; }

        [Required]
        public DateTime DataZamowienia { get; set; } = DateTime.Now;

        [Required]
        public string SzczegolyZamowienia { get; set; }

        [Required]
        public string DaneOdbiorcy { get; set; }

        [Required]
        public decimal Suma { get; set; }
       
        [Required]
        [MaxLength(50)]
        public string Platnosc { get; set; } = "Płatność przy odbiorze";

        [Required]
        public string DaneUzytkownika { get; set; } // Kolumna w bazie danych
    }
}
