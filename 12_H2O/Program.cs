const int NumOxygen = 4;
const int NumHydrogen = 8;

SemaphoreSlim mutex = new(1, 1);
SemaphoreSlim oxygenQueue = new(0, int.MaxValue);
SemaphoreSlim hydrogenQueue = new(0, int.MaxValue);

SemaphoreSlim barrierMutex = new(1, 1);
SemaphoreSlim barrierGate = new(0, int.MaxValue);
int bondedCount = 0;

int oxygenWaiting = 0;
int hydrogenWaiting = 0;
int moleculeNumber = 0;

void Bond(string role, int id)
{
    Console.WriteLine($"  {role}-{id} vezuje se u molekul.");
}

void MoleculeBarrier()
{
    barrierMutex.Wait();
    bondedCount++;

    if (bondedCount == 3)
    {
        for (int i = 0; i < 3; i++)
        {
            barrierGate.Release();
        }
        bondedCount = 0;
    }

    barrierMutex.Release();
    barrierGate.Wait();
}

void OxygenThread(object? arg)
{
    int id = (int)arg!;
    bool isLeader = false;

    mutex.Wait();
    oxygenWaiting++;

    if (hydrogenWaiting >= 2)
    {
        hydrogenQueue.Release();
        hydrogenQueue.Release();
        hydrogenWaiting -= 2;

        oxygenWaiting--;
        isLeader = true;
    }
    else
    {
        mutex.Release();
    }

    if (!isLeader)
    {
        oxygenQueue.Wait();
    }

    Bond("O", id);
    MoleculeBarrier();

    if (isLeader)
    {
        moleculeNumber++;
        Console.WriteLine($"Molekul H2O broj {moleculeNumber} formiran.");
        mutex.Release();
    }
}

void HydrogenThread(object? arg)
{
    int id = (int)arg!;
    bool isLeader = false;

    mutex.Wait();
    hydrogenWaiting++;

    if (hydrogenWaiting >= 2 && oxygenWaiting >= 1)
    {
        hydrogenQueue.Release();
        hydrogenWaiting -= 2;

        oxygenQueue.Release();
        oxygenWaiting--;

        isLeader = true;
    }
    else
    {
        mutex.Release();
    }

    if (!isLeader)
    {
        hydrogenQueue.Wait();
    }

    Bond("H", id);
    MoleculeBarrier();

    if (isLeader)
    {
        moleculeNumber++;
        Console.WriteLine($"Molekul H2O broj {moleculeNumber} formiran.");
        mutex.Release();
    }
}

Thread[] oxygenThreads = new Thread[NumOxygen];
Thread[] hydrogenThreads = new Thread[NumHydrogen];

for (int i = 0; i < NumHydrogen; i++)
{
    hydrogenThreads[i] = new Thread(HydrogenThread);
    hydrogenThreads[i].Start(i + 1);
}

for (int i = 0; i < NumOxygen; i++)
{
    oxygenThreads[i] = new Thread(OxygenThread);
    oxygenThreads[i].Start(i + 1);
}

foreach (Thread t in hydrogenThreads)
{
    t.Join();
}

foreach (Thread t in oxygenThreads)
{
    t.Join();
}

Console.WriteLine($"\nUkupno formirano {moleculeNumber} molekula vode.");
