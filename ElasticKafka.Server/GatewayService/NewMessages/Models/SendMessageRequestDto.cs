namespace GatewayService.NewMessages.Models;

public sealed record SendMessageRequestDto(
    string MessageText,
    DateTimeOffset SentAt);