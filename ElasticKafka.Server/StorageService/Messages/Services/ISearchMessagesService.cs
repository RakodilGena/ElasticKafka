using StorageService.Messages.Models;
using StorageService.Messages.Models.Requests;

namespace StorageService.Messages.Services;

public interface ISearchMessagesService
{
    Task<IReadOnlyList<MessageDto>> SearchMessagesAsync(
        SearchMessagesRequestDto request,
        CancellationToken cancellationToken);
}