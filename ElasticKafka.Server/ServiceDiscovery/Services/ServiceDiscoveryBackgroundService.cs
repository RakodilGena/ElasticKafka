namespace ServiceDiscovery.Services;

internal sealed class ServiceDiscoveryBackgroundService : BackgroundService
{
    private readonly ServiceUrlsStreamer _serviceDiscovery;
    private readonly ILogger<ServiceDiscoveryBackgroundService> _logger;

    public ServiceDiscoveryBackgroundService(
        ServiceUrlsStreamer serviceDiscovery,
        ILogger<ServiceDiscoveryBackgroundService> logger)
    {
        _serviceDiscovery = serviceDiscovery;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting serviceDiscoveryBackgroundService");

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await _serviceDiscovery.BroadcastServiceUrlsAsync();

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Error occured while broadcasting service urls");

                if (stoppingToken.IsCancellationRequested)
                    return;

                await Task.Delay(TimeSpan.FromSeconds(1), CancellationToken.None);
            }
    }
}