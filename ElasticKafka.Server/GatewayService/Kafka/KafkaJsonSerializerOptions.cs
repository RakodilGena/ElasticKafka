using System.Text.Json;

namespace GatewayService.Kafka;

internal static class KafkaJsonSerializerOptions
{
    public static JsonSerializerOptions Default => new();
}