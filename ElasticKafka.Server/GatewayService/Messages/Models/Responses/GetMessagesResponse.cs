namespace GatewayService.Messages.Models.Responses;

public sealed record GetMessagesResponse(IReadOnlyList<MessageDto> Messages);