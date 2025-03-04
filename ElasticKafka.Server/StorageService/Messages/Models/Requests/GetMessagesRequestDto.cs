namespace StorageService.Messages.Models.Requests;

public sealed record GetMessagesRequestDto(
    int Count,
    int Offset);