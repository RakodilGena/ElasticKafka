using MessagingService.Kafka.Producers.Impl;

namespace MessagingService.Kafka.Producers;

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
            .AddScoped<IMessageProducer, MessageProducer>();
        
        return services;
    }
}