using StorageService.Messages.Models;
using StorageService.Messages.Models.Requests;

namespace StorageService.Messages.Services;

public interface IGetMessagesService
{
    Task<IReadOnlyList<MessageDto>> GetMessagesAsync(
        GetMessagesRequestDto request,
        CancellationToken cancellationToken);
}