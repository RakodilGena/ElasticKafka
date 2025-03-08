using System.ComponentModel.DataAnnotations;
using Confluent.Kafka;

namespace StorageService.Kafka.Consumers.NewMessages.Config;

internal sealed class NewMessagesConsumerConfig
{
    public const string SectionName = "Kafka:Consumers:NewMessages";

    [Required] public string Topic { get; init; } = null!;
    [Required] public ConsumerConfig Config { get; init; } = null!;
}