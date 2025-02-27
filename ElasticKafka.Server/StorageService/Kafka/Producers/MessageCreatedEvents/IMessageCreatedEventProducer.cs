namespace StorageService.Kafka.Producers.MessageCreatedEvents;

public interface IMessageCreatedEventProducer
{
    Task ProduceAsync(Guid messageId);
}