namespace GatewayService.Messages.Models.Requests;

public sealed record SearchMessagesRequest(
    int Count,
    int Offset,
    string Filter);