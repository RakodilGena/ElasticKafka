using ServiceDiscovery.Models;

namespace ServiceDiscovery.Extensions;

internal static class ReaderWriterLockSlimExtensions
{
    public static ReaderWriterLockSlimReadHook EnterReadHook(this ReaderWriterLockSlim @lock)
    {
        @lock.EnterReadLock();
        return new ReaderWriterLockSlimReadHook(@lock);
    }
    
    public static ReaderWriterLockSlimWriteHook EnterWriteHook(this ReaderWriterLockSlim @lock)
    {
        @lock.EnterWriteLock();
        return new ReaderWriterLockSlimWriteHook(@lock);
    }
}