using Confluent.Kafka;

namespace StorageService.Kafka.Consumers.NewMessages.Config;

internal sealed class NewMessagesConsumerConfig
{
    public const string SectionName = "Kafka:Consumers:NewMessages";
    
    public string Topic { get; init; } = null!;
    public ConsumerConfig Config { get; init; } = null!;
}