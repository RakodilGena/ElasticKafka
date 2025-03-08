namespace ServiceDiscovery.Models;

internal readonly struct ReaderWriterLockSlimReadHook : IDisposable
{
    private readonly ReaderWriterLockSlim _lock;

    public ReaderWriterLockSlimReadHook(ReaderWriterLockSlim @lock)
    {
        _lock = @lock;
    }

    public void Dispose()
    {
        _lock.ExitReadLock();
    }
}