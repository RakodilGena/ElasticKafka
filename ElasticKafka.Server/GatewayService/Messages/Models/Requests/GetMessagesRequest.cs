namespace GatewayService.Messages.Models.Requests;

public sealed record GetMessagesRequest(
        int Count,
        int Offset);