using StorageService.Messages.Services.Impl;

namespace StorageService.Messages.Services;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICreateMessageService, CreateMessageService>();
    }
}