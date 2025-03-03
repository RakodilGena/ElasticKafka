namespace GatewayService.Messages.Models.Requests;

public sealed record SearchMessagesRequestDto(
    int Count,
    int Offset,
    string Filter);