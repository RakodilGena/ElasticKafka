using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using ServiceDiscovery.Options;

namespace ServiceDiscovery.Services;

internal sealed class ServiceUrlsStreamer : ServiceDiscoveryRpc.ServiceDiscoveryRpcBase
{
    private readonly List<Listener> _listeners = [];
    private readonly Lock _lock = new();
    private readonly IOptionsMonitor<ServiceUrls> _serviceUrlsMonitor;
    private readonly ILogger<ServiceUrlsStreamer> _logger;

    public ServiceUrlsStreamer(
        IOptionsMonitor<ServiceUrls> serviceUrlsMonitor,
        ILogger<ServiceUrlsStreamer> logger)
    {
        _serviceUrlsMonitor = serviceUrlsMonitor;
        _logger = logger;
    }

    public override async Task ListenToServiceUrlsStream(
        Empty request,
        IServerStreamWriter<ServiceUrlsRpc> responseStream,
        ServerCallContext context)
    {
        Listener listener;

        lock (_lock)
        {
            listener = new Listener(
                Guid.CreateVersion7(),
                responseStream);

            _listeners.Add(listener);
            _logger.LogInformation("Added new listener [{id}]", listener.Id);
        }

        try
        {
            //freeze every listener unless it cancels.
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(Timeout.Infinite, context.CancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            //remove listener on cancellation.
            lock (_lock)
            {
                _listeners.Remove(listener);
                _logger.LogInformation("removed listener [{id}]", listener.Id);
            }
        }
    }

    public async Task BroadcastServiceUrlsAsync()
    {
        _logger.LogInformation("Broadcasting service urls...");

        Listener[] currentListeners;
        lock (_lock)
        {
            currentListeners = _listeners.ToArray();
        }

        if (currentListeners.Length is 0)
        {
            _logger.LogInformation("No listeners found, skipping.");
            return;
        }

        var serviceUrls = BuildUrlsRpc();

        foreach (var listener in currentListeners)
            try
            {
                await listener.StreamWriter.WriteAsync(serviceUrls);
            }
            catch (Exception e)
            {
                var listenerId = listener.Id;
                lock (_lock)
                {
                    _listeners.Remove(listener);
                    _logger.LogInformation(
                        "removed listener [{id}] on error",
                        listenerId);
                }

                _logger.LogError(e,
                    "Error while broadcasting service urls to listener [{listenerId}]",
                    listenerId);
            }

        _logger.LogInformation("Successfully broadcast service urls");
    }

    private ServiceUrlsRpc BuildUrlsRpc()
    {
        var urls = _serviceUrlsMonitor.CurrentValue;

        _logger.LogInformation(
            "To broadcast: Messaging services: [{messagingServices}], Storage services: [{storageServices}]",
            urls.MessagingServices,
            urls.StorageServices);

        var messagingServicesUrls = urls.MessagingServices.Split(",");
        var storageServicesUrls = urls.StorageServices.Split(",");

        var serviceList = new ServiceUrlsRpc
        {
            StorageServices = { storageServicesUrls },
            MessagingServices = { messagingServicesUrls }
        };

        return serviceList;
    }

    private readonly record struct Listener(
        Guid Id,
        IServerStreamWriter<ServiceUrlsRpc> StreamWriter);
}