using Confluent.Kafka;

namespace StorageService.Kafka.Producers.Config;

internal sealed class KafkaProducerConfig
{
    public const string SectionName = "Kafka:Producer";
    
    public string MessageCreatedEventsTopic { get; set; } = null!;
    
    public ProducerConfig Config { get; set; } = null!;
}