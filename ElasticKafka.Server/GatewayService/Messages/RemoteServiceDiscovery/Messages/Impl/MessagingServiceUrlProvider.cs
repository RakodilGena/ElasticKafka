using Microsoft.Extensions.Options;

namespace GatewayService.Messages.RemoteServiceDiscovery.Messages.Impl;

internal sealed class MessagingServiceUrlProvider : IMessagingServiceUrlProvider
{
    private readonly IOptionsSnapshot<MessageServicesUrls> _options;

    public MessagingServiceUrlProvider(
        IOptionsSnapshot<MessageServicesUrls> options)
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