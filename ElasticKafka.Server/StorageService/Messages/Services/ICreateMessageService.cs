using StorageService.Messages.Models.Requests;

namespace StorageService.Messages.Services;

internal interface ICreateMessageService
{
    Task<bool> TryCreateMessageAsync(
        CreateMessageRequestDto request,
        CancellationToken cancellationToken);
}