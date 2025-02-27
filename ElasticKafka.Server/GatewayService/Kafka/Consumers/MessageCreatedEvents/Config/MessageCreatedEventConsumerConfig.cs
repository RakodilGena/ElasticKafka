using Confluent.Kafka;

namespace GatewayService.Kafka.Consumers.MessageCreatedEvents.Config;

internal sealed class MessageCreatedEventConsumerConfig
{
    public const string SectionName = "Kafka:Consumers:MessageCreatedEvents";
    
    public string Topic { get; init; } = null!;
    public ConsumerConfig Config { get; init; } = null!;
}