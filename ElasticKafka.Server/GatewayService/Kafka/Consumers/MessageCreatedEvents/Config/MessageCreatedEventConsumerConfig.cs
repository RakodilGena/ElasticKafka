using System.ComponentModel.DataAnnotations;
using Confluent.Kafka;

namespace GatewayService.Kafka.Consumers.MessageCreatedEvents.Config;

internal sealed class MessageCreatedEventConsumerConfig
{
    public const string SectionName = "Kafka:Consumers:MessageCreatedEvents";

    [Required] public string Topic { get; init; } = null!;
    [Required] public ConsumerConfig Config { get; init; } = null!;
}