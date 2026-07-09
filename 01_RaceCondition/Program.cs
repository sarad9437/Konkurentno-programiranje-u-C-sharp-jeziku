const int NumThreads = 4;
const int NumIncrements = 1_000_000;

long counter = 0;

void IncrementCounter()
{
    for (int i = 0; i < NumIncrements; i++)
    {
        counter++;
    }
}

Thread[] threads = new Thread[NumThreads];

for (int i = 0; i < NumThreads; i++)
{
    threads[i] = new Thread(IncrementCounter);
    threads[i].Start();
}

for (int i = 0; i < NumThreads; i++)
{
    threads[i].Join();
}

Console.WriteLine($"Ocekivana vrednost: {NumThreads * NumIncrements}");
Console.WriteLine($"Dobijena vrednost:  {counter}");
