using System.ComponentModel.DataAnnotations;
using Confluent.Kafka;

namespace StorageService.Kafka.Producers.Config;

internal sealed class KafkaProducerConfig
{
    public const string SectionName = "Kafka:Producer";

    [Required] public string MessageCreatedEventsTopic { get; set; } = null!;

    [Required] public ProducerConfig Config { get; set; } = null!;
}