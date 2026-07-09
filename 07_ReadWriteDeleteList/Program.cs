const int N = 8;

ThreadSafeLinkedList list = new();
object consoleLock = new();

list.Insert(2);
list.Insert(5);
list.Insert(7);

void Worker(object? args)
{
    int id = (int)args!;
    Random random = new();

    for (int i = 0; i < 20; i++)
    {
        int val = random.Next(10);
        int op = random.Next(3);
        string action;
        int result;

        switch (op)
        {
            case 0:
                result = list.Insert(val);
                action = result == 1 ? $"Thread {id}: insert({val}) - uspesno dodato" : $"Thread {id}: insert({val}) - vec postoji";
                break;
            case 1:
                result = list.Contains(val);
                action = result == 1 ? $"Thread {id}: contains({val}) - pronadjeno" : $"Thread {id}: contains({val}) - ne postoji";
                break;
            default:
                result = list.Delete(val);
                action = result == 1 ? $"Thread {id}: delete({val}) - uspesno obrisano" : $"Thread {id}: delete({val}) - ne postoji";
                break;
        }

        string listState = list.GetContents();

        lock (consoleLock)
        {
            Console.WriteLine($"{action} - lista: {listState}");
        }
    }
}

Thread[] threads = new Thread[N];

for (int i = 0; i < N; i++)
{
    threads[i] = new Thread(Worker);
    threads[i].Start(i);
}

foreach (Thread t in threads)
{
    t.Join();
}

class Node
{
    public int Val;
    public Node? Next;
}

class ThreadSafeLinkedList
{
    private Node? head;

    private readonly SemaphoreSlim mutex = new(1, 1);
    private readonly SemaphoreSlim turnstile = new(1, 1);
    private readonly SemaphoreSlim rwdLock = new(1, 1);
    private int readCount = 0;

    private void ReadLock()
    {
        turnstile.Wait();
        turnstile.Release();

        mutex.Wait();
        readCount++;
        if (readCount == 1)
        {
            rwdLock.Wait();
        }
        mutex.Release();
    }

    private void ReadUnlock()
    {
        mutex.Wait();
        readCount--;
        if (readCount == 0)
        {
            rwdLock.Release();
        }
        mutex.Release();
    }

    private void UpdateLock()
    {
        turnstile.Wait();
        rwdLock.Wait();
    }

    private void UpdateUnlock()
    {
        turnstile.Release();
        rwdLock.Release();
    }

    public int Contains(int val)
    {
        ReadLock();

        Node? curr = head;
        while (curr != null && curr.Val < val)
        {
            curr = curr.Next;
        }

        int result = (curr != null && curr.Val == val) ? 1 : 0;
        ReadUnlock();

        return result;
    }

    public int Insert(int val)
    {
        UpdateLock();

        Node? curr = head;
        Node? prev = null;
        while (curr != null && curr.Val < val)
        {
            prev = curr;
            curr = curr.Next;
        }

        if (curr != null && curr.Val == val)
        {
            UpdateUnlock();
            return 0;
        }

        Node temp = new() { Val = val, Next = curr };

        if (prev == null)
        {
            head = temp;
        }
        else
        {
            prev.Next = temp;
        }

        UpdateUnlock();
        return 1;
    }

    public int Delete(int val)
    {
        UpdateLock();

        Node? curr = head;
        Node? prev = null;
        while (curr != null && curr.Val < val)
        {
            prev = curr;
            curr = curr.Next;
        }

        if (curr == null || curr.Val != val)
        {
            UpdateUnlock();
            return 0;
        }

        if (prev == null)
        {
            head = curr.Next;
        }
        else
        {
            prev.Next = curr.Next;
        }

        UpdateUnlock();
        return 1;
    }

    public string GetContents()
    {
        ReadLock();

        var parts = new System.Collections.Generic.List<string>();
        Node? curr = head;
        while (curr != null)
        {
            parts.Add(curr.Val.ToString());
            curr = curr.Next;
        }

        ReadUnlock();

        return "{ " + string.Join(", ", parts) + " }";
    }
}
