const int N = 5;

SemaphoreSlim[] stapici = new SemaphoreSlim[N];

for (int i = 0; i < N; i++)
{
    stapici[i] = new SemaphoreSlim(1, 1);
}

void Filozof(object? arg)
{
    int id = (int)arg!;

    int levi = id;
    int desni = (id + 1) % N;

    for (int i = 0; i < 3; i++)
    {
        Console.WriteLine($"Filozof {id} razmislja");
        Thread.Sleep(200);

        Console.WriteLine($"Filozof {id} je gladan i pokusace da uzme stapice");

        if (id == N - 1)
        {
            Console.WriteLine($"Filozof {id} ceka desni stapic broj [{desni}]");
            stapici[desni].Wait();

            Console.WriteLine($"Filozof {id} ceka levi stapic broj [{levi}]");
            stapici[levi].Wait();
        }
        else
        {
            Console.WriteLine($"Filozof {id} ceka levi stapic broj [{levi}]");
            stapici[levi].Wait();

            Console.WriteLine($"Filozof {id} ceka desni stapic broj [{desni}]");
            stapici[desni].Wait();
        }

        Console.WriteLine($"Filozof {id} je uzeo oba stapica [{levi},{desni}] i pocinje sa jelom");
        Thread.Sleep(300);

        stapici[levi].Release();
        stapici[desni].Release();

        Console.WriteLine($"Filozof {id} je zavrsio sa jelom i vratio je stapice");
        Thread.Sleep(100);
    }
}

Thread[] filozofi = new Thread[N];

for (int i = 0; i < N; i++)
{
    filozofi[i] = new Thread(Filozof);
    filozofi[i].Start(i);
}

foreach (Thread filozof in filozofi)
{
    filozof.Join();
}
