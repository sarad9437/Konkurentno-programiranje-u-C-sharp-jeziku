const int ReaderCount = 5;
const int WriterCount = 2;
const int ReaderLoops = 3;
const int WriterLoops = 3;
const string FileName = "shared.txt";

SemaphoreSlim roomEmpty = new(1, 1);
SemaphoreSlim turnstile = new(1, 1);
SemaphoreSlim readCountMutex = new(1, 1);

int readCount = 0;

void InitializeFile()
{
    File.WriteAllText(FileName, "Pocetni sadrzaj deljenog resursa.\n");
}

void Reader(object? arg)
{
    int id = (int)arg!;

    for (int i = 0; i < ReaderLoops; i++)
    {
        turnstile.Wait();
        turnstile.Release();

        readCountMutex.Wait();
        readCount++;

        if (readCount == 1)
        {
            roomEmpty.Wait();
        }

        readCountMutex.Release();

        Console.WriteLine($"\nReader {id} pocinje citanje.");

        foreach (string line in File.ReadLines(FileName))
        {
            Console.WriteLine($"Reader {id} procitao: {line}");
        }

        Console.WriteLine($"Reader {id} zavrsio citanje.\n");

        readCountMutex.Wait();

        readCount--;

        if (readCount == 0)
        {
            roomEmpty.Release();
        }

        readCountMutex.Release();

        Thread.Sleep(100 + Random.Shared.Next(300));
    }
}

void Writer(object? arg)
{
    int id = (int)arg!;

    for (int i = 0; i < WriterLoops; i++)
    {
        turnstile.Wait();

        roomEmpty.Wait();

        File.AppendAllText(FileName, $"Writer {id} je upisao liniju {i + 1}\n");
        Console.WriteLine($"Writer {id} upisao liniju {i + 1} u resurs.");

        roomEmpty.Release();
        turnstile.Release();

        Thread.Sleep(150 + Random.Shared.Next(350));
    }
}

InitializeFile();

Thread[] readers = new Thread[ReaderCount];
Thread[] writers = new Thread[WriterCount];

for (int i = 0; i < ReaderCount; i++)
{
    readers[i] = new Thread(Reader);
    readers[i].Start(i + 1);
}

for (int i = 0; i < WriterCount; i++)
{
    writers[i] = new Thread(Writer);
    writers[i].Start(i + 1);
}

foreach (Thread reader in readers)
{
    reader.Join();
}

foreach (Thread writer in writers)
{
    writer.Join();
}
