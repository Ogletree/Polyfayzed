using System.Collections.Concurrent;

namespace AspireApp.ApiService.Strategy;

public class LockManager
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public async Task<bool> TryAcquireLockAsync(string lockName)
    {
        var semaphore = _locks.GetOrAdd(lockName, _ => new SemaphoreSlim(1, 1));
        return await semaphore.WaitAsync(0);
    }

    public void ReleaseLock(string lockName)
    {
        if (_locks.TryGetValue(lockName, out var semaphore))
        {
            semaphore.Release();
        }
    }
}