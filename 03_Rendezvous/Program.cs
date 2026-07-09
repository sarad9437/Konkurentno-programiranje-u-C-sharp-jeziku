SemaphoreSlim aDone = new(0, 1);
SemaphoreSlim bDone = new(0, 1);

void ThreadA()
{
    Console.WriteLine("A: a1");
    aDone.Release();

    bDone.Wait();

    Console.WriteLine("A: a2");
}

void ThreadB()
{
    Console.WriteLine("B: b1");

    bDone.Release();

    aDone.Wait();

    Console.WriteLine("B: b2");
}

Thread threadA = new(ThreadA);
Thread threadB = new(ThreadB);

threadA.Start();
threadB.Start();

threadA.Join();
threadB.Join();
