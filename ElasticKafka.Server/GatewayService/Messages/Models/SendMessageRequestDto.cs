namespace GatewayService.Messages.Models;

public sealed record SendMessageRequestDto(
    string MessageText,
    DateTimeOffset SentAt);