namespace GatewayService.Messages.Models.Responses;

public sealed record SearchMessagesResponse(IReadOnlyList<MessageDto> Messages);