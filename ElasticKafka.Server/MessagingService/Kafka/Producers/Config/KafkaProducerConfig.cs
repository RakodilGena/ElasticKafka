using System.ComponentModel.DataAnnotations;
using Confluent.Kafka;

namespace MessagingService.Kafka.Producers.Config;

internal sealed class KafkaProducerConfig
{
    public const string SectionName = "Kafka:Producer";

    [Required] public string NewMessagesTopic { get; set; } = null!;

    [Required] public ProducerConfig Config { get; set; } = null!;
}