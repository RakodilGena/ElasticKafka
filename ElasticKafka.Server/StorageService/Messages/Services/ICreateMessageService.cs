using StorageService.Kafka.Consumers.NewMessages.Models;

namespace StorageService.Messages.Services;

internal interface ICreateMessageService
{
    Task<bool> TryCreateMessageAsync(
        KafkaNewMessage kafkaNewMessage,
        CancellationToken cancellationToken);
}