namespace GatewayService.Messages.RemoteServiceDiscovery.Consumers;

internal interface IServiceDiscoveryClient
{
    Task StartListeningAsync(CancellationToken cancellationToken);
}