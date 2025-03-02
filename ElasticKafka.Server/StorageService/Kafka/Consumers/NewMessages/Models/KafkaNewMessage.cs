using JetBrains.Annotations;

namespace StorageService.Kafka.Consumers.NewMessages.Models;

[UsedImplicitly]
internal sealed record KafkaNewMessage(
    Guid Id,
    string Text,
    DateTimeOffset SentAt);