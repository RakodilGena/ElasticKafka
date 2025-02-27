using Confluent.Kafka;

namespace MessagingService.Kafka.Producers;

internal interface IProducerProvider
{
    IProducer<string, string> Get();
}