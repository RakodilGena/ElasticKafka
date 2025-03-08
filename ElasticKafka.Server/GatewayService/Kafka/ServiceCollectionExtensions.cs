using GatewayService.Kafka.Consumers;
using GatewayService.Kafka.Consumers.Impl;
using GatewayService.Kafka.Consumers.MessageCreatedEvents;
using GatewayService.Kafka.Consumers.MessageCreatedEvents.Config;

namespace GatewayService.Kafka;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services)
    {
        services
            .AddOptions<MessageCreatedEventConsumerConfig>()
            .Configure<IConfiguration>(
                (opt, config) =>
                    config
                        .GetSection(MessageCreatedEventConsumerConfig.SectionName)
                        .Bind(opt))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddSingleton<IConsumerProvider, ConsumerProvider>()
            .AddHostedService<MessageCreatedEventsConsumer>();

        return services;
    }
}