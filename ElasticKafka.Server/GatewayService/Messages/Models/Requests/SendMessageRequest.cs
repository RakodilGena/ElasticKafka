namespace GatewayService.Messages.Models.Requests;

public sealed record SendMessageRequest(
    string MessageText,
    DateTimeOffset SentAt);