const int ReaderCount = 6;
const int ReaderRounds = 8;

SemaphoreSlim rwMutex = new(1, 1);
SemaphoreSlim readCountMutex = new(1, 1);

int readCount = 0;
bool keepReadersRunning = true;

void Reader(object? arg)
{
    int id = (int)arg!;

    for (int round = 0; round < ReaderRounds && keepReadersRunning; round++)
    {
        readCountMutex.Wait();
        readCount++;

        if (readCount == 1)
        {
            rwMutex.Wait();
        }
        readCountMutex.Release();

        Console.WriteLine($"Reader {id} cita (runda {round + 1}).");
        Thread.Sleep(80);

        readCountMutex.Wait();
        readCount--;

        if (readCount == 0)
        {
            rwMutex.Release();
        }
        readCountMutex.Release();

        Thread.Sleep(20);
    }
}

void WriterOnce()
{
    Console.WriteLine("\nWriter zeli da pise i pokusava da udje...\n");

    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    rwMutex.Wait();

    stopwatch.Stop();

    Console.WriteLine($"\nWriter je usao i pise. Cekao je {stopwatch.ElapsedMilliseconds} ms.\n");
    Thread.Sleep(50);

    rwMutex.Release();

    keepReadersRunning = false;
}

Thread[] readers = new Thread[ReaderCount];

for (int i = 0; i < ReaderCount; i++)
{
    readers[i] = new Thread(Reader);
    readers[i].Start(i + 1);
}

Thread.Sleep(100);

Thread writerThread = new(WriterOnce);
writerThread.Start();

writerThread.Join();

foreach (Thread reader in readers)
{
    reader.Join();
}

Console.WriteLine("Writer je morao da ceka jer su reader niti pristizale bez prestanka.");
