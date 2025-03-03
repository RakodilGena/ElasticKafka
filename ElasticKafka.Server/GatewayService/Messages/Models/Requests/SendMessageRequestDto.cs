namespace GatewayService.Messages.Models.Requests;

public sealed record SendMessageRequestDto(
    string MessageText,
    DateTimeOffset SentAt);