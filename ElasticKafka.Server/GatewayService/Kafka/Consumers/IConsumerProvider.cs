using Confluent.Kafka;

namespace GatewayService.Kafka.Consumers;

internal interface IConsumerProvider
{
    IConsumer<string, string> Create(ConsumerConfig config);
}