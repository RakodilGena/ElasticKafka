using StorageService.Messages.Services.Impl;

namespace StorageService.Messages.Services;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageServices(this IServiceCollection services)
    {
        //possible to create these as singletons as loggers and elasticclient are singletones
        return services
            .AddSingleton<ICreateMessageService, CreateMessageService>()
            .AddSingleton<IDeleteMessageService, DeleteMessageService>()
            .AddSingleton<IGetMessagesService, GetMessagesService>()
            .AddSingleton<ISearchMessagesService, SearchMessagesService>();
    }
}