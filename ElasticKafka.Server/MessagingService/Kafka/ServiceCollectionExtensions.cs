using MessagingService.Kafka.Producers;
using MessagingService.Kafka.Producers.Config;
using MessagingService.Kafka.Producers.Impl;
using MessagingService.Kafka.Producers.NewMessages;
using MessagingService.Kafka.Producers.NewMessages.Impl;

namespace MessagingService.Kafka;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services)
    {
        services
            .AddOptions<KafkaProducerConfig>()
            .Configure<IConfiguration>(
                (opt, config) =>
                    config
                        .GetSection(KafkaProducerConfig.SectionName)
                        .Bind(opt));
        
        services
            .AddSingleton<IProducerProvider, ProducerProvider>()
            .AddScoped<INewMessageProducer, NewMessageProducer>();
        
        return services;
    }
}