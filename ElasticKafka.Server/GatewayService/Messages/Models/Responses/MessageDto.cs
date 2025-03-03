namespace GatewayService.Messages.Models.Responses;

public sealed record MessageDto(Guid Id, string Text, DateTimeOffset SentAt);