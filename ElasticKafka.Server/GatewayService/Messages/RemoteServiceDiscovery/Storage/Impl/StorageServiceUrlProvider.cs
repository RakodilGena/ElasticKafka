namespace GatewayService.Messages.RemoteServiceDiscovery.Storage.Impl;

internal sealed class StorageServiceUrlProvider : 
    IStorageServiceUrlProvider,
    IStorageServiceUrlCollection
{
    private string[] _urls = [];
    private int _currentUrlIndex = -1;
    
    private readonly ReaderWriterLockSlim _lock = new();
    
    public string GetUrl()
    {
        try
        {
            _lock.EnterReadLock();
            
            if (_urls.Length is 0)
            {
                throw new Exception("No urls discovered yet.");
            }
            _currentUrlIndex = (_currentUrlIndex + 1) % _urls.Length;
            return _urls[_currentUrlIndex];
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void ApplyUrls(IEnumerable<string> urls)
    {
        try
        {
            _lock.EnterWriteLock();
            _urls = urls.ToArray();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
}