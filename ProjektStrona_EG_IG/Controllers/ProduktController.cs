using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektStrona_EG_IG.Areas.Identity.Data;
using ProjektStrona_EG_IG.Models;

namespace ProjektStrona_EG_IG.Controllers
{
    public class ProduktController : Controller
    {
        // Wstrzyknięcie kontekstu bazy danych
        private readonly AppDbContext _context;

        public ProduktController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ProduktController
        public ActionResult Index()
        {
            var produkty = _context.Produkt.ToList();
            return View(produkty);
        }

        // GET: ProduktController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProduktController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProduktController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Produkt produkt)
        {
            try
            {
                if (ModelState.IsValid) // Sprawdzanie poprawności modelu
                {
                    _context.Produkt.Add(produkt); // Dodanie produktu do bazy
                    _context.SaveChanges(); // Zapisanie zmian w bazie
                    return RedirectToAction(nameof(Index)); // Powrót do listy produktów
                }
                return View(produkt); // Jeśli model nie jest poprawny, wróć do widoku Create
            }
            catch
            {
                return View(produkt); // Jeśli wystąpił błąd, wróć do widoku Create
            }
        }
        // GET: ProduktController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProduktController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Koszyk()
        {
            // Pobieramy wszystkie elementy koszyka i dołączamy informacje o użytkowniku oraz produkcie
            var koszyki = _context.Koszyk
                .Include(k => k.Uzytkownik)  // Dołączamy dane użytkownika
                .Include(k => k.Produkt)     // Dołączamy dane produktu
                .ToList();

            return View(koszyki);  // Przekazujemy dane do widoku
        }

        [HttpPost]
        public IActionResult AddToKoszyk(int produktId, int ilosc)
        {
            try
            {
                // Pobierz produkt z bazy danych
                var produkt = _context.Produkt.FirstOrDefault(p => p.Id == produktId);
                if (produkt == null || produkt.IloscDostepna < ilosc)
                {
                    return NotFound("Produkt nie istnieje lub dostępna ilość jest niewystarczająca.");
                }

                // Zmniejsz ilość dostępną
                produkt.IloscDostepna -= ilosc;

                // Dodaj do koszyka
                var koszyk = new Koszyk
                {
                    ProduktId = produktId,
                    UzytkownikId = 1, // Dla testów ustawiamy ID użytkownika na 1 (zmień w przyszłości na aktualnego użytkownika)
                    Ilosc = ilosc
                };

                _context.Koszyk.Add(koszyk);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult RemoveFromKoszyk(int koszykId)
        {
            var koszykItem = _context.Koszyk.FirstOrDefault(k => k.Id == koszykId);
            if (koszykItem != null)
            {
                // Zwiększ ilość dostępnego produktu
                var produkt = _context.Produkt.FirstOrDefault(p => p.Id == koszykItem.ProduktId);
                if (produkt != null)
                {
                    produkt.IloscDostepna += koszykItem.Ilosc;
                }

                // Usuń element z koszyka
                _context.Koszyk.Remove(koszykItem);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Koszyk));
        }

        // GET: ProduktController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProduktController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }

        }
    }
}