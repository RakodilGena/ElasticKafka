namespace ElasticKafka.Client.Messaging;

internal sealed record SendMessageRequest(
    string? MessageText,
    DateTimeOffset? SentAt);