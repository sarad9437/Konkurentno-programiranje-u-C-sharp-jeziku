const int BoatSize = 4;
const int NumHackers = 12;
const int NumSerfs = 12;

SemaphoreSlim mutex = new(1, 1);
SemaphoreSlim hackerQueue = new(0, int.MaxValue);
SemaphoreSlim serfQueue = new(0, int.MaxValue);

SemaphoreSlim barrierMutex = new(1, 1);
SemaphoreSlim barrierGate = new(0, int.MaxValue);
int boardedCount = 0;

int hackersWaiting = 0;
int serfsWaiting = 0;

void Board(string role, int id)
{
    Console.WriteLine($"  {role} {id} se ukrcava u camac.");
}

void RowBoat(string role, int id)
{
    Console.WriteLine($"{role} {id} vesla. Camac isplovljava sa 4 putnika.");
}

void BoatBarrier()
{
    barrierMutex.Wait();

    boardedCount++;

    if (boardedCount == BoatSize)
    {
        for (int i = 0; i < BoatSize; i++)
        {
            barrierGate.Release();
        }

        boardedCount = 0;
    }

    barrierMutex.Release();

    barrierGate.Wait();
}

void HackerThread(object? arg)
{
    int id = (int)arg!;
    bool isCaptain = false;

    Thread.Sleep(Random.Shared.Next(500));

    mutex.Wait();
    hackersWaiting++;

    if (hackersWaiting == 4)
    {
        hackerQueue.Release();
        hackerQueue.Release();
        hackerQueue.Release();

        hackersWaiting = 0;
        isCaptain = true;
    }
    else if (hackersWaiting == 2 && serfsWaiting >= 2)
    {
        hackerQueue.Release();
        serfQueue.Release();
        serfQueue.Release();

        serfsWaiting -= 2;
        hackersWaiting = 0;
        isCaptain = true;
    }
    else
    {
        mutex.Release();
    }

    if (!isCaptain)
    {
        hackerQueue.Wait();
    }

    Board("Haker", id);
    BoatBarrier();

    if (isCaptain)
    {
        RowBoat("Haker", id);
        mutex.Release();
    }
}

void SerfThread(object? arg)
{
    int id = (int)arg!;
    bool isCaptain = false;

    Thread.Sleep(Random.Shared.Next(500));

    mutex.Wait();
    serfsWaiting++;

    if (serfsWaiting == 4)
    {
        serfQueue.Release();
        serfQueue.Release();
        serfQueue.Release();

        serfsWaiting = 0;
        isCaptain = true;
    }
    else if (serfsWaiting == 2 && hackersWaiting >= 2)
    {
        serfQueue.Release();
        hackerQueue.Release();
        hackerQueue.Release();

        hackersWaiting -= 2;
        serfsWaiting = 0;
        isCaptain = true;
    }
    else
    {
        mutex.Release();
    }

    if (!isCaptain)
    {
        serfQueue.Wait();
    }

    Board("Serf", id);
    BoatBarrier();

    if (isCaptain)
    {
        RowBoat("Serf", id);
        mutex.Release();
    }
}

Thread[] hackers = new Thread[NumHackers];
Thread[] serfs = new Thread[NumSerfs];

for (int i = 0; i < NumHackers; i++)
{
    hackers[i] = new Thread(HackerThread);
    hackers[i].Start(i + 1);
}

for (int i = 0; i < NumSerfs; i++)
{
    serfs[i] = new Thread(SerfThread);
    serfs[i].Start(i + 1);
}

foreach (Thread hacker in hackers)
{
    hacker.Join();
}

foreach (Thread serf in serfs)
{
    serf.Join();
}

Console.WriteLine("\nSvi hakeri i serfovi su prebaceni preko reke.");
