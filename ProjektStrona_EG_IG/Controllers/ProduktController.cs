using System.Security.Claims;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
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
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrWhiteSpace(produkt.ZdjecieUrl))
                    {
                        ModelState.AddModelError("ZdjecieUrl", "Zdjęcie produktu jest wymagane.");
                        return View(produkt);
                    }

                    _context.Produkt.Add(produkt);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                return View(produkt);
            }
            catch
            {
                return View(produkt);
            }
        }

        //GET EDIT
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var produkt = _context.Produkt.FirstOrDefault(p => p.Id == id);
            if (produkt == null)
            {
                return NotFound("Produkt nie został znaleziony.");
            }
            return View(produkt);
        }

        //POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Produkt produkt)
        {
            if (id != produkt.Id)
            {
                return BadRequest("Nieprawidłowe ID produktu.");
            }

            if (string.IsNullOrWhiteSpace(produkt.ZdjecieUrl))
            {
                ModelState.AddModelError("ZdjecieUrl", "Zdjęcie produktu jest wymagane.");
            }

            if (ModelState.IsValid)
            {
                _context.Entry(produkt).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(produkt);
        }

        //GET DELETE
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var produkt = _context.Produkt.FirstOrDefault(p => p.Id == id);
            if (produkt == null)
            {
                return NotFound("Produkt nie został znaleziony.");
            }
            return View(produkt);
        }

        //POST DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            var produkt = _context.Produkt.FirstOrDefault(p => p.Id == id);
            if (produkt != null)
            {
                _context.Produkt.Remove(produkt);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Koszyk()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Pobiera ID zalogowanego użytkownika
            if (userId == null) //Brak autoryzacji jeżeli ID jest puste
            {
                return View("Error1");
            }

            var koszyki = _context.Koszyk //odwołanie do tabeli Koszyk w bazie
                .Where(k => k.Uzytkownik.AppUserId == userId) //Wyświetla tylko koszyk zalogowanego użytkownika
                .Include(k => k.Produkt)
                .ToList();

            foreach (var koszyk in koszyki)
            {
                if (koszyk.Produkt == null || string.IsNullOrWhiteSpace(koszyk.Produkt.ZdjecieUrl))
                {
                    koszyk.Produkt.ZdjecieUrl = "/images/default.png";
                }
            }

            return View(koszyki);
        }

        [HttpPost]
        public IActionResult AddToKoszyk(int produktId, int ilosc)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Pobiera ID zalogowanego użytkownika
                if (userId == null)  //Brak autoryzacji jeżeli ID jest puste
                {
                    return View("Error1");
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

                var koszyk = _context.Koszyk.FirstOrDefault(k => k.UzytkownikId == uzytkownik.Id && k.ProduktId == produktId);
                if (koszyk != null)
                {
                    koszyk.Ilosc += ilosc;
                }
                else
                {
                    koszyk = new Koszyk
                    {
                        ProduktId = produktId,
                        UzytkownikId = uzytkownik.Id, //Przypisanie do zalogowanego użytkownika
                        Ilosc = ilosc
                    };
                    _context.Koszyk.Add(koszyk); //Dodaje nowy element do koszyka użytkownika w bazie danych
                }

                _context.SaveChanges(); //Zapisanie zmian
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
            try
            {
                //Wyszukaj pozycję w koszyku na podstawie przekazanego ID koszyka
                var pozycjaKoszyka = _context.Koszyk
                    .Include(k => k.Produkt)
                    .FirstOrDefault(k => k.Id == koszykId);

                if (pozycjaKoszyka == null)
                {
                    return NotFound("Pozycja koszyka nie została znaleziona.");
                }

                //Zwiększenie ilości dostępnych produktów o zwróconą ilość
                pozycjaKoszyka.Produkt.IloscDostepna += pozycjaKoszyka.Ilosc;

                //Usuwanie pozycji koszyka z bazy danych
                _context.Koszyk.Remove(pozycjaKoszyka);

                _context.SaveChanges();

                return RedirectToAction(nameof(Koszyk));
            }
            catch
            {
                return BadRequest("Wystąpił błąd podczas usuwania pozycji z koszyka.");
            }
        }
        [HttpPost]
        public IActionResult Zamow()
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                //Pobranie ID zalogowanego użytkownika
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized();

                //Wyszukanie użytkownika w bazie danych
                var uzytkownik = _context.Uzytkownik.FirstOrDefault(u => u.AppUserId == userId);
                if (uzytkownik == null || uzytkownik.Imie == "Wprowadź imię" || uzytkownik.Nazwisko == "Wprowadź nazwisko" || uzytkownik.Adres == "Wprowadź adres" || uzytkownik.KodPocztowy == "00-000") return View("Error2");

                //Pobieranie koszyka użytkownika
                var koszyk = _context.Koszyk
                    .Where(k => k.UzytkownikId == uzytkownik.Id)
                    .Include(k => k.Produkt)
                    .ToList();

                if (!koszyk.Any()) return BadRequest("Koszyk jest pusty.");

                //Obliczenie sumy zamówienia
                decimal sumaZamowienia = koszyk.Sum(k => k.Ilosc * k.Produkt.Cena);

                //Przygotowanie szczegółów zamówienia
                var szczegoly = string.Join(", ", koszyk.Select(k => $"{k.Produkt.Nazwa}, Ilość: {k.Ilosc}"));
                var daneOdbiorcy = $"Imię i Nazwisko: {uzytkownik.Imie} {uzytkownik.Nazwisko}, Adres: {uzytkownik.Adres}, Kod Pocztowy: {uzytkownik.KodPocztowy}, Telefon {uzytkownik.Telefon}";

                //Utworzenie nowego zamówienia
                var zamowienie = new Zamowienie
                {
                    UzytkownikId = uzytkownik.Id,
                    DataZamowienia = DateTime.Now,
                    SzczegolyZamowienia = szczegoly,
                    DaneOdbiorcy = daneOdbiorcy,
                    Suma = sumaZamowienia,
                    DaneUzytkownika = daneOdbiorcy,
                    Platnosc = "Płatność przy odbiorze"
                };

                _context.Zamowienia.Add(zamowienie);

                //Usunięcie pozycji koszyka
                _context.Koszyk.RemoveRange(koszyk);
                _context.SaveChanges();
                transaction.Commit();

                return RedirectToAction("HistoriaTransakcji");
            }
            catch
            {
                transaction.Rollback();
                return BadRequest("Wystąpił błąd podczas realizacji zamówienia.");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult KoszykAdmin()
        {
            var zamowienia = _context.Koszyk //odwołanie do tabeli Koszyk w bazie
                .Include(k => k.Uzytkownik) //Spis wszystkich zamówień
                .Include(k => k.Produkt)
                .ToList();

            return View(zamowienia);
        }

        public IActionResult HistoriaTransakcji()
        {
            //Pobranie ID zalogowanego użytkownika
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return View("Error1");

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
        public ActionResult DaneUzytkownika()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Pobiera ID zalogowanego użytkownika
            if (userId == null)
            {
                return View("Error1");
            }

            var uzytkownik = _context.Uzytkownik.FirstOrDefault(u => u.AppUserId == userId);
            if (uzytkownik == null)
            {
                return NotFound("Użytkownik nie został znaleziony.");
            }

            return View(uzytkownik);
        }

        //POST: ProduktController/DaneUzytkownika
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DaneUzytkownika(Uzytkownik model)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized();
                }

                var uzytkownik = _context.Uzytkownik.FirstOrDefault(u => u.AppUserId == userId);
                if (uzytkownik == null)
                {
                    return NotFound("Użytkownik nie został znaleziony.");
                }

                //Aktualizacja danych użytkownika
                uzytkownik.Imie = model.Imie;
                uzytkownik.Nazwisko = model.Nazwisko;
                uzytkownik.Adres = model.Adres;
                uzytkownik.KodPocztowy = model.KodPocztowy;
                uzytkownik.Telefon = model.Telefon;

                _context.SaveChanges(); //Zapis zmian w bazie danych

                return RedirectToAction(nameof(Index)); //Powrót na stronę główną lub inną
            }
            catch
            {
                return View(model); //W przypadku błędu zwróć formularz z danymi
            }
        }

    }
}