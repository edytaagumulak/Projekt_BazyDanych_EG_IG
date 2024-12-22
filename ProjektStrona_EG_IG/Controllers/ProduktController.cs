using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektStrona_EG_IG.Areas.Identity.Data;
using ProjektStrona_EG_IG.Models;

namespace ProjektStrona_EG_IG.Controllers
{
    public class ProduktController : Controller
    {
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
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProduktController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Produkt produkt)
        {
            try
            {
                if (ModelState.IsValid) //Walidacja modelu
                {
                    _context.Produkt.Add(produkt); //Dodanie produktu do bazy
                    _context.SaveChanges(); //Zapisanie zmian w bazie
                    return RedirectToAction(nameof(Index)); //Powrót do listy produktów
                }
                return View(produkt); //Jeśli model nie jest poprawny, powrót do widoku Create
            }
            catch
            {
                return View(produkt); //Jeśli wystąpił błąd, powrót do widoku Create
            }
        }

        // GET: ProduktController/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProduktController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Pobiera ID zalogowanego użytkownika
            if (userId == null) //Brak autoryzacji jeżeli ID jest puste
            {
                return Unauthorized();
            }

            var koszyki = _context.Koszyk   //odwołanie do tabeli Koszyk w bazie
                .Where(k => k.Uzytkownik.AppUserId == userId) //Wyświetla tylko koszyk zalogowanego użytkownika
                .Include(k => k.Produkt)
                .ToList();

            return View(koszyki);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ZamowieniaAdmin()
        {
            var zamowienia = _context.Koszyk //odwołanie do tabeli Koszyk w bazie
                .Include(k => k.Uzytkownik) //Spis wszystkich zamówień
                .Include(k => k.Produkt)
                .ToList();

            return View(zamowienia);
        }



        [HttpPost]
        public IActionResult AddToKoszyk(int produktId, int ilosc)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Pobiera ID zalogowanego użytkownika
                if (userId == null)
                {
                    return Unauthorized(); //Brak autoryzacji jeżeli ID jest puste
                }

                //Wyszukanie użytkownika w bazie danych (AppUserId)
                var uzytkownik = _context.Uzytkownik.FirstOrDefault(u => u.AppUserId == userId);
                if (uzytkownik == null)
                {
                    return NotFound("Użytkownik nie został znaleziony.");
                }

                //Wyszukuje produkt w bazie na podstawie jego ID
                var produkt = _context.Produkt.FirstOrDefault(p => p.Id == produktId);
                if (produkt == null || produkt.IloscDostepna < ilosc)
                {
                    return NotFound("Produkt nie istnieje lub dostępna ilość jest niewystarczająca.");
                }

                //Zmniejsza ilość produktow w bazie o zamówioną ilość
                produkt.IloscDostepna -= ilosc;

                var koszyk = new Koszyk
                {
                    ProduktId = produktId,
                    UzytkownikId = uzytkownik.Id, //Przypisanie do zalogowanego użytkownika
                    Ilosc = ilosc
                };

                _context.Koszyk.Add(koszyk); //Dodaje nowy element do koszyka użytkownika w bazie danych
                _context.SaveChanges(); //Zapisanie zmian

                return RedirectToAction(nameof(Koszyk));
            }
            catch
            {
                return RedirectToAction(nameof(Koszyk));
            }
        }



        [HttpPost]
        public IActionResult RemoveFromKoszyk(int koszykId)
        {
            //Wyszukaj pozycję w koszyku na podstawie przekazanego ID koszyka
            var koszykItem = _context.Koszyk.FirstOrDefault(k => k.Id == koszykId);
            if (koszykItem != null)
            {
                //Wyszukaj produkty związane z danym koszykiem
                var produkt = _context.Produkt.FirstOrDefault(p => p.Id == koszykItem.ProduktId);
                if (produkt != null)
                {
                    //Zwiększenie ilości dostępnych produktów o zwróconą ilość
                    produkt.IloscDostepna += koszykItem.Ilosc;
                }

                //Usuwanie pozycji koszyka z bazy danych
                _context.Koszyk.Remove(koszykItem);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Koszyk));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Zamow()
        {
            //Rozpoczęcie transakcji bazy danych
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                //Pobranie ID zalogowanego użytkownika
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized();

                //Wyszukanie użytkownika w bazie danych
                var uzytkownik = _context.Uzytkownik.FirstOrDefault(u => u.AppUserId == userId);
                if (uzytkownik == null) return NotFound("Nie znaleziono użytkownika.");

                //Pobieranie koszyka użytkownika
                var koszyk = _context.Koszyk
                    .Where(k => k.UzytkownikId == uzytkownik.Id)
                    .Include(k => k.Produkt)
                    .ToList();

                //W przypadku, gdy koszyk jest pusty zwróci komunikat
                if (!koszyk.Any()) return BadRequest("Koszyk jest pusty.");

                //Oblicza sumę zamówienia
                decimal sumaZamowienia = koszyk.Sum(k => k.Ilosc * k.Produkt.Cena);

                //Przygotowanie szczegółów zamówienia
                var szczegoly = string.Join(", ",
                    koszyk.Select(k => $"{k.Produkt.Nazwa} x{k.Ilosc}"));

                //Utworzenie nowego zamówienia
                var zamowienie = new Zamowienie
                {
                    UzytkownikId = uzytkownik.Id,
                    DataZamowienia = DateTime.Now,
                    SzczegolyZamowienia = szczegoly,
                    Suma = sumaZamowienia
                };

                //Dodanie nowego zamówienia do bazy
                _context.Zamowienia.Add(zamowienie);

                //Usunięcie pozycji koszyka
                _context.Koszyk.RemoveRange(koszyk);

                _context.SaveChanges();
                transaction.Commit();

                return RedirectToAction("HistoriaTransakcji");
            }
            catch
            {
                //Rollback w przypadku, gdy wystąpi błąd
                transaction.Rollback();
                return BadRequest("Wystąpił błąd podczas realizacji zamówienia.");
            }
        }


        public IActionResult HistoriaTransakcji()
        {
            //Pobranie ID zalogowanego użytkownika
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            //Wyszukanie użytkownika w bazie po ID
            var uzytkownik = _context.Uzytkownik.FirstOrDefault(u => u.AppUserId == userId);
            if (uzytkownik == null) return NotFound("Nie znaleziono użytkownika.");

            //Pobranie wszystkich zamówień użytkownika, dodatkowo są posortowane po dacie zamówienia w porządku malejącym
            var zamowienia = _context.Zamowienia
                .Where(z => z.UzytkownikId == uzytkownik.Id)
                .OrderByDescending(z => z.DataZamowienia)
                .ToList();

            return View(zamowienia);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult HistoriaAdmin()
        {
            //Wyszukanie wszystkich zamówień użytkowników
            var zamowienia = _context.Zamowienia
                .Include(z => z.Uzytkownik)
                .OrderByDescending(z => z.DataZamowienia)
                .ToList();

            return View(zamowienia);
        }

        // GET: ProduktController/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProduktController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

