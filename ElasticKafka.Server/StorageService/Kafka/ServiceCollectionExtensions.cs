using StorageService.Kafka.NewMessages;
using StorageService.Kafka.NewMessages.Config;
using StorageService.Kafka.NewMessages.Impl;

namespace StorageService.Kafka;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services)
    {
        services
            .AddOptions<NewMessagesConsumerConfig>()
            .Configure<IConfiguration>(
                (opt, config) =>
                    config
                        .GetSection(NewMessagesConsumerConfig.SectionName)
                        .Bind(opt));
        
        services
            .AddSingleton<IConsumerProvider, ConsumerProvider>()
            .AddHostedService<NewMessagesConsumer>();

        return services;
    }
}