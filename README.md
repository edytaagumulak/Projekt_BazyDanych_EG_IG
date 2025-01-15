# **Drogeria internetowa - Kosmoteria** #

**Edyta Gumulak**  
Studentka III roku kierunku, Informatyka  
I stopień, studia stacjonarne  
Nr albumu: 160946  
Grupa L6  

**Iza Grzyb**  
Studentka III roku kierunku, Informatyka  
I stopień, studia stacjonarne  
Nr albumu: 160945  
Grupa L6  

---
# **Opis działania aplikacji** #

Istnieje możliwość rejestracji i zalogowania.

Wszyscy użytkownicy mogą przeglądać produkty na stronie. 

Tylko zalogowani użytkownicy mogą dodawać produkty do koszyka oraz je usuwać (z koszyka), a także edytować swoje dane personalne.

Koszyk działa na zasadzie rezerwacji produktu, produkt znajduje się w koszyku do momentu zakupu lub usunięcia go.

Administrator może dodawać nowe produkty, edytować je oraz usuwać. Posiada również podgląd do wszystkich koszyków oraz do historii zamówień klientów. 

---

# **Testowi użytkownicy** #

* **Administrator:** admin@gmail.com   Admin123!  
* **Zwykły użytkownik:** user@gmail.com   User123!  

---

# **Wymagania** #
System operacyjny: Windows 10/11 z obsługą .NET 8.0.


# **Oprogramowanie** #
.NET SDK 8.0 (https://dotnet.microsoft.com/download/dotnet/8.0)  
SQL Server (lub LocalDB).


# **Pakiety NuGet zainstalowane w projekcie** #
* Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.11)  
* Microsoft.AspNetCore.Identity.UI (8.0.11)  
* Microsoft.EntityFrameworkCore.Tools  (8.0.11)  
* Microsoft.EntityFrameworkCore.SqlServer  (8.0.11)  
* Microsoft.VisualStudio.Web.CodeGeneration.Design (8.0.7)  


# **Proces instalacji** #
**Sklonuj repozytorium projektu:**  
git clone https://github.com/edytaagumulak/Projekt_BazyDanych_EG_IG.git     

**Przejdź do folderu projektu:**  
cd <nazwa_folderu_projektu>  

**Przywróć zależności NuGet:**  
dotnet restore  

### **Przygotowanie bazy danych**  
**Uruchom program SQL Server Management Studio**  

**Utwórz bazę danych o nazwie "ProjektStrona_EG_IG"**  
CREATE DATABASE ProjektStrona_EG_IG;  

**Connection String zawarty w aplikacji:**  
"ConnectionStrings": {  
    "AppDbContextConnection": "Server=EDIII\\SQLEXPRESS;Database=ProjektStrona_EG_IG;Trusted_Connection=True;TrustServerCertificate=true"  
}  

**Utwórz migrację i zaaktualizuj baze danych**  
add-migration <nazwa>  
update-database  
