using Confluent.Kafka;

namespace StorageService.Kafka.NewMessages;

internal interface IConsumerProvider
{
    IConsumer<string, string> Create(ConsumerConfig config);
}