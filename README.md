# Konkurentno programiranje u C# jeziku kroz primere sa vežbi

Seminarski rad iz predmeta Konkurentno i distribuirano programiranje.

## Struktura

| # | Projekat | Opis |
|---|---|---|
| 01 | RaceCondition | Demonstracija race condition-a nad deljenim brojačem bez sinhronizacije |
| 02 | MutexKarte | Check-Then-Act problem (prodaja karte) rešen pomoću lock mehanizma |
| 03 | Rendezvous | Koordinacija redosleda dve niti pomoću dva semafora |
| 04 | Barrier | N niti čeka jedna drugu na zajedničkoj tački pre nastavka |
| 05 | ReadersWritersStarvation | Osnovno readers-writers rešenje sa demonstracijom writer starvation-a |
| 06 | ReadersWritersNoStarvation | Readers-writers sa turnstile mehanizmom koji sprečava starvation |
| 07 | ReadWriteDeleteList | Thread-safe sortirana povezana lista sa insert/contains/delete operacijama |
| 08 | DiningPhilosophers | Nesimetrično rešenje problema filozofa koji večeraju, sprečava deadlock |
| 09 | CigaretteSmokers | Agent stavlja 2 od 3 sastojka, odgovarajući pušač pravi cigaretu |
| 10 | SleepingBarber | Berberin spava dok nema mušterija, čekaonica sa ograničenim kapacitetom |
| 11 | RiverCrossing | Formiranje validnih posada (4H, 4S ili 2H+2S) za prelazak reke |
| 12 | H2O | Formiranje molekula vode (2H + 1O) korišćenjem scoreboard i barijera |

## Pokretanje

Otvoriti `KonkurentnoProgramiranje.sln` u Visual Studio-u.

Za pokretanje pojedinačnog projekta: desni klik na projekat u Solution Explorer-u →
Set as Startup Project → Ctrl+F5.

Alternativno, iz terminala:

```
cd 01_RaceCondition
dotnet run
```
