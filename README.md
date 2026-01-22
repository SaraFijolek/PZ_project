# Pharmacy Management System – Back-end (.NET C#)

Aby zalogować się jako administrator, sprawdź dane logowania w pliku `appsettings.json`.

Adres API dla front-endu: `https://localhost:7220`

# Uruchomienie API : 
to  trzeba  mieć  visual studio i sql  server .
Ważne  trzeba  w  pliku appsettings.json zmienić  "DefaultConnection": "Twoja nazwa bazy danych" i zrobić  dwa poleceniania w konsoli w vs :

Add-migration NazwaMigracji 
a potem 
Update-database

# Testy

### Uruchamianie
Pharmacy_Test -> Dependencies -> Add Project Reference... <br />
Test -> Test Explorer -> Run All Tests In View

> [!IMPORTANT]
> Testy wymagają pakietów NuGet
> ![Description](https://github.com/SaraFijolek/PZ_project/blob/tests/nuget.png)
