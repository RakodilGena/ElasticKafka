namespace GatewayService.Messages.Models;

public sealed record SendMessageRequest(
    string? MessageText,
    DateTimeOffset? SentAt);