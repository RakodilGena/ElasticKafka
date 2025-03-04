namespace StorageService.Messages.Models;

public sealed record MessageDto(
    Guid Id,
    string Text,
    DateTimeOffset SentAt);