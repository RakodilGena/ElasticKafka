using GatewayService.Messages.RemoteServiceDiscovery.Messages;
using GatewayService.Messages.RemoteServiceDiscovery.Storage;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ServiceDiscovery;

namespace GatewayService.Messages.RemoteServiceDiscovery.Consumers.Impl;

internal sealed class ServiceDiscoveryClient : IServiceDiscoveryClient
{
    private readonly IMessagingServiceUrlCollection _messagingServiceUrlCollection;
    private readonly IStorageServiceUrlCollection _storageServiceUrlCollection;
    private readonly ServiceDiscoveryRpc.ServiceDiscoveryRpcClient _client;
    private readonly ILogger<ServiceDiscoveryClient> _logger;

    public ServiceDiscoveryClient(
        IMessagingServiceUrlCollection messagingServiceUrlCollection,
        IStorageServiceUrlCollection storageServiceUrlCollection,
        ServiceDiscoveryRpc.ServiceDiscoveryRpcClient client,
        ILogger<ServiceDiscoveryClient> logger)
    {
        _messagingServiceUrlCollection = messagingServiceUrlCollection;
        _storageServiceUrlCollection = storageServiceUrlCollection;
        _client = client;
        _logger = logger;
    }

    public async Task StartListeningAsync(CancellationToken cancellationToken)
    {
        using var call = _client.ListenToServiceUrlsStream(
            new Empty(),
            cancellationToken: cancellationToken);

        try
        {
            await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
            {
                if (response is null)
                {
                    _logger.LogWarning("Received empty response from ServiceDiscovery");
                    continue;
                }

                _storageServiceUrlCollection.ApplyUrls(response.StorageServices);
                _messagingServiceUrlCollection.ApplyUrls(response.MessagingServices);

                _logger.LogInformation(
                    "Received response from ServiceDiscovery. Messaging services: [{messagingServices}]. Storage services: [{storageServices}].",
                    string.Join(", ", response.MessagingServices),
                    string.Join(", ", response.StorageServices));
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Stopping listening to ServiceDiscovery");
        }
    }
}