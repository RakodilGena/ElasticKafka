using JetBrains.Annotations;

namespace StorageService.Kafka.NewMessages.Models;

[UsedImplicitly]
internal sealed record KafkaNewMessage( 
    string Id,
    string Text);