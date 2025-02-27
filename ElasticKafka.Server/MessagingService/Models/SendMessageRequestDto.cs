namespace MessagingService.Models;

public sealed record SendMessageRequestDto(
    Guid MessageId,
    string MessageText);