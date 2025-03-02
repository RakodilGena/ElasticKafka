namespace GatewayService.NewMessages.Models;

public sealed record SendMessageRequest(
    string? MessageText,
    DateTimeOffset? SentAt);