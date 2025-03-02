using GatewayService.Messages.RemoteServiceDiscovery.Messages;
using GatewayService.Messages.RemoteServiceDiscovery.Messages.Impl;
using GatewayService.Messages.RemoteServiceDiscovery.Storage;
using GatewayService.Messages.RemoteServiceDiscovery.Storage.Impl;

namespace GatewayService.Messages.RemoteServiceDiscovery;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceDiscovery(this IServiceCollection services)
    {
        services.AddOptions<MessageServicesUrls>()
            .BindConfiguration(MessageServicesUrls.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<StorageServicesUrls>()
            .BindConfiguration(StorageServicesUrls.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        return services
            .AddScoped<IMessagingServiceUrlProvider, MessagingServiceUrlProvider>()
            .AddScoped<IStorageServiceUrlProvider, StorageServiceUrlProvider>();
    }
}