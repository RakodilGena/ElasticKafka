using Confluent.Kafka;

namespace StorageService.Kafka.Producers;

internal interface IProducerProvider
{
    IProducer<string, string> Get();
}