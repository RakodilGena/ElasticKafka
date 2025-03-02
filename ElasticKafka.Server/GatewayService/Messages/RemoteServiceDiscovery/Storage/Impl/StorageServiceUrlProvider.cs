using Microsoft.Extensions.Options;

namespace GatewayService.Messages.RemoteServiceDiscovery.Storage.Impl;

internal sealed class StorageServiceUrlProvider : IStorageServiceUrlProvider
{
    private readonly IOptionsSnapshot<StorageServicesUrls> _options;

    public StorageServiceUrlProvider(
        IOptionsSnapshot<StorageServicesUrls> options)
    {
        _options = options;
    }
    
    public string GetUrl()
    {
        var urlString = _options.Value.Value;
        
        var urls = urlString.Split(",");
        
        if (urls.Length is 1)
            return urls[0];
        
        var idx = Random.Shared.Next(0, urls.Length);
        
        return urls[idx];
    }
}