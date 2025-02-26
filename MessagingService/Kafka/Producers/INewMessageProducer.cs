using MessagingService.Models;

namespace MessagingService.Kafka.Producers;

public interface INewMessageProducer
{
    Task ProduceAsync(SendMessageRequestDto request);
}