const int Rounds = 6;

SemaphoreSlim agentSem = new(1, 1);
SemaphoreSlim[] smokerSem = [new(0, 1), new(0, 1), new(0, 1)];

string[] smokerHas = ["duvan", "papir", "sibice"];
int[] smokerCounts = [0, 0, 0];

void Agent()
{
    Random random = new();

    for (int i = 0; i < Rounds; i++)
    {
        agentSem.Wait();

        int choice = random.Next(3);
        smokerCounts[choice]++;

        if (choice == 0)
        {
            Console.WriteLine("Agent stavlja papir i sibice na sto");
            smokerSem[0].Release();
        }
        else if (choice == 1)
        {
            Console.WriteLine("Agent stavlja duvan i sibice na sto");
            smokerSem[1].Release();
        }
        else
        {
            Console.WriteLine("Agent stavlja duvan i papir na sto");
            smokerSem[2].Release();
        }
    }
}

void Smoker(object? arg)
{
    int id = (int)arg!;

    while (true)
    {
        if (!smokerSem[id].Wait(TimeSpan.FromSeconds(3)))
            break;

        Console.WriteLine($"Pusac {id} (ima {smokerHas[id]}) pravi cigaretu i pusi");

        Thread.Sleep(300);

        agentSem.Release();
    }
}

Thread agentThread = new(Agent);
Thread[] smokers = new Thread[3];

agentThread.Start();

for (int i = 0; i < 3; i++)
{
    smokers[i] = new Thread(Smoker);
    smokers[i].Start(i);
}

agentThread.Join();

foreach (Thread s in smokers)
{
    s.Join();
}

Console.WriteLine($"\nZavrseno {Rounds} rundi.");
