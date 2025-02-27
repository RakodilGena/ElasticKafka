using JetBrains.Annotations;

namespace StorageService.Kafka.Consumers.NewMessages.Models;

[UsedImplicitly]
internal sealed record KafkaNewMessage( 
    string Id,
    string Text);