namespace ServiceDiscovery.Models;

internal readonly struct ReaderWriterLockSlimWriteHook : IDisposable
{
    private readonly ReaderWriterLockSlim _lock;

    public ReaderWriterLockSlimWriteHook(ReaderWriterLockSlim @lock)
    {
        _lock = @lock;
    }

    public void Dispose()
    {
        _lock.ExitWriteLock();
    }
}