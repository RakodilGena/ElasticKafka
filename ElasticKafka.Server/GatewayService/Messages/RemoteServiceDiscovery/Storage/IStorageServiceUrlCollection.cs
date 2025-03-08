namespace GatewayService.Messages.RemoteServiceDiscovery.Storage;

internal interface IStorageServiceUrlCollection
{
    void ApplyUrls(IEnumerable<string> urls);
}