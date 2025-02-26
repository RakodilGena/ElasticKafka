using GatewayService.Messages.Services.Impl;
using GatewayService.Messages.Setup;

namespace GatewayService.Messages.Services;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageServices(this IServiceCollection services)
    {
        services.AddOptions<MessageServicesUrls>()
            .BindConfiguration(MessageServicesUrls.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services
            .AddScoped<IMessageService, MessageService>()
            .AddScoped<IMessageServiceUrlProvider, MessageServiceUrlProvider>();
        
        return services;
    }
}