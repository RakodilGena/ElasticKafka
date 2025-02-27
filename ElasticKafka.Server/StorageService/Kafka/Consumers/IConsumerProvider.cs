using Confluent.Kafka;

namespace StorageService.Kafka.Consumers;

internal interface IConsumerProvider
{
    IConsumer<string, string> Create(ConsumerConfig config);
}