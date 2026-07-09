const int WaitingRoomSize = 3;
const int TotalCustomers = 10;

SemaphoreSlim customerReady = new(0, int.MaxValue);
SemaphoreSlim barberReady = new(0, 1);
SemaphoreSlim mutex = new(1, 1);

int waitingCustomers = 0;
int servedCustomers = 0;
int totalSeated = 0;

void Barber()
{
    while (true)
    {
        if (!customerReady.Wait(TimeSpan.FromSeconds(3)))
            break;

        mutex.Wait();
        waitingCustomers--;
        servedCustomers++;
        barberReady.Release();
        mutex.Release();

        Console.WriteLine($"Berberin sisa musteriju. (preostalo u cekaonici: {waitingCustomers})");
        Thread.Sleep(300);
        Console.WriteLine("Berberin je zavrsio sisanje.");
    }
}

void Customer(object? arg)
{
    int id = (int)arg!;

    Thread.Sleep(Random.Shared.Next(800));
    Console.WriteLine($"Musterija {id} dolazi u berbernicu.");

    mutex.Wait();

    if (waitingCustomers < WaitingRoomSize)
    {
        waitingCustomers++;
        Console.WriteLine($"Musterija {id} seda u cekaonicu. (ukupno ceka: {waitingCustomers})");

        customerReady.Release();
        mutex.Release();

        barberReady.Wait();
        Console.WriteLine($"Musterija {id} je na redu i biva usluzen.");
    }
    else
    {
        mutex.Release();
        Console.WriteLine($"Musterija {id} odlazi (nema slobodnog mesta).");
    }
}

Thread barberThread = new(Barber);
barberThread.Start();

Thread[] customers = new Thread[TotalCustomers];

for (int i = 0; i < TotalCustomers; i++)
{
    customers[i] = new Thread(Customer);
    customers[i].Start(i + 1);
}

foreach (Thread c in customers)
{
    c.Join();
}

barberThread.Join();

Console.WriteLine($"\nUsluzeno {servedCustomers} od {TotalCustomers} musterija.");
