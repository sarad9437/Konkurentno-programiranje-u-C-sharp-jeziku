const int NumKarata = 1;

int preostaleKarte = NumKarata;

object mutexKarte = new();

void ProdajKartu(object? arg)
{
    string imeProdavca = (string)arg!;

    lock (mutexKarte)
    {
        if (preostaleKarte > 0)
        {
            Thread.Sleep(10);

            preostaleKarte--;

            Console.WriteLine($"{imeProdavca} je uspesno kupio kartu. Preostalo karata: {preostaleKarte}");
        }
        else
        {
            Console.WriteLine($"{imeProdavca} nije uspeo da kupi kartu jer nema vise karata.");
        }
    }
}

Console.WriteLine($"Pocetno stanje: {preostaleKarte} karta(e) dostupno.\n");

Thread prodavac1 = new(ProdajKartu);
Thread prodavac2 = new(ProdajKartu);

prodavac1.Start("Prodavac A");
prodavac2.Start("Prodavac B");

prodavac1.Join();
prodavac2.Join();

Console.WriteLine($"\nKonacno stanje: {preostaleKarte} preostalih karata.");
