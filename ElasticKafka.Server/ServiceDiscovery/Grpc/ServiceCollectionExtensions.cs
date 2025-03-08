using ServiceDiscovery.Options;

namespace ServiceDiscovery.Grpc;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceDiscovery(
        this IServiceCollection services)
    {
        services.AddOptions<ServiceUrls>()
            .BindConfiguration(ServiceUrls.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ServiceUrlsStreamer>();

        services.AddHostedService<ServiceDiscoveryBackgroundService>();

        return services;
    }
}