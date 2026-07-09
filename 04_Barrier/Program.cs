const int ThreadCount = 5;

SemaphoreSlim countMutex = new(1, 1);
SemaphoreSlim barrierSem = new(0, int.MaxValue);

int arrivedCount = 0;

void BarrierPoint()
{
    countMutex.Wait();

    arrivedCount++;

    if (arrivedCount == ThreadCount)
    {
        for (int i = 0; i < ThreadCount; i++)
        {
            barrierSem.Release();
        }
    }

    countMutex.Release();

    barrierSem.Wait();
}

void Worker(object? arg)
{
    int id = (int)arg!;

    Console.WriteLine($"Thread {id}: Prvi deo posla");
    Thread.Sleep(Random.Shared.Next(500));

    BarrierPoint();

    Console.WriteLine($"Thread {id}: Drugi deo posla");
}

Thread[] threads = new Thread[ThreadCount];

for (int i = 0; i < ThreadCount; i++)
{
    threads[i] = new Thread(Worker);
    threads[i].Start(i);
}

foreach (Thread t in threads)
{
    t.Join();
}
