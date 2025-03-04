using StorageService.Messages.Models.Requests;
using StorageService.Messages.Models.Responses;

namespace StorageService.Messages.Services;

public interface IDeleteMessageService
{
    Task<DeleteMessageResponseDto> DeleteMessageAsync(DeleteMessageRequestDto request);
}