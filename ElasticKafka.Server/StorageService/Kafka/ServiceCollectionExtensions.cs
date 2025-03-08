using StorageService.Kafka.Consumers;
using StorageService.Kafka.Consumers.Impl;
using StorageService.Kafka.Consumers.NewMessages;
using StorageService.Kafka.Consumers.NewMessages.Config;
using StorageService.Kafka.Producers;
using StorageService.Kafka.Producers.Config;
using StorageService.Kafka.Producers.Impl;
using StorageService.Kafka.Producers.MessageCreatedEvents;
using StorageService.Kafka.Producers.MessageCreatedEvents.Impl;

namespace StorageService.Kafka;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafka(
        this IServiceCollection services,
        bool inMigratorMode)
    {
        if (inMigratorMode)
            return services;


        //consumers---------------------------------------
        services
            .AddOptions<NewMessagesConsumerConfig>()
            .Configure<IConfiguration>(
                (opt, config) =>
                    config
                        .GetSection(NewMessagesConsumerConfig.SectionName)
                        .Bind(opt))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddSingleton<IConsumerProvider, ConsumerProvider>()
            .AddHostedService<NewMessagesConsumer>();


        //producers---------------------------------------
        services
            .AddOptions<KafkaProducerConfig>()
            .Configure<IConfiguration>(
                (opt, config) =>
                    config
                        .GetSection(KafkaProducerConfig.SectionName)
                        .Bind(opt))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddSingleton<IProducerProvider, ProducerProvider>()
            .AddSingleton<IMessageCreatedEventProducer, MessageCreatedEventProducer>();

        return services;
    }
}