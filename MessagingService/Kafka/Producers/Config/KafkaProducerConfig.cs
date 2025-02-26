using Confluent.Kafka;

namespace MessagingService.Kafka.Producers.Config;

internal sealed class KafkaProducerConfig
{
    public const string SectionName = "Kafka:Producer";
    
    public string NewMessagesTopic { get; set; } = null!;
    
    public ProducerConfig Config { get; set; } = null!;
}