namespace StorageService.Messages.Models.Requests;

internal sealed record CreateMessageRequestDto(
    Guid MessageId,
    string MessageText,
    DateTimeOffset MessageSentAt);