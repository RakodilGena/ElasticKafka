namespace GatewayService.Messages.RemoteServiceDiscovery.Consumers;

internal sealed class ServiceDiscoveryBackgroundService : BackgroundService
{
    private readonly IServiceDiscoveryClient _serviceDiscoveryClient;
    private readonly ILogger<ServiceDiscoveryBackgroundService> _logger;

    public ServiceDiscoveryBackgroundService(
        IServiceDiscoveryClient serviceDiscoveryClient,
        ILogger<ServiceDiscoveryBackgroundService> logger)
    {
        _serviceDiscoveryClient = serviceDiscoveryClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting listening to ServiceDiscovery");

                await _serviceDiscoveryClient.StartListeningAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while listening to ServiceDiscovery");

                if (stoppingToken.IsCancellationRequested)
                    return;

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}