using System.Text.Json;

namespace MessagingService.Kafka;

internal static class KafkaJsonSerializerOptions
{
    public static JsonSerializerOptions Default => new();
}