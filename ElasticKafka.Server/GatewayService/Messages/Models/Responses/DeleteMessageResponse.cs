namespace GatewayService.Messages.Models.Responses;

public sealed record DeleteMessageResponse(
    bool Success,
    string? Reason);