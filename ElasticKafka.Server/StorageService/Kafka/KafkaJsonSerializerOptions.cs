using System.Text.Json;

namespace StorageService.Kafka;

internal static class KafkaJsonSerializerOptions
{
    public static JsonSerializerOptions Default => new();
}