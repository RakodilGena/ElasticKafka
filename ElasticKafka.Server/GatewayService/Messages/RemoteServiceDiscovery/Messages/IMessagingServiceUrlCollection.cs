namespace GatewayService.Messages.RemoteServiceDiscovery.Messages;

internal interface IMessagingServiceUrlCollection
{
    void ApplyUrls(IEnumerable<string> urls);
}