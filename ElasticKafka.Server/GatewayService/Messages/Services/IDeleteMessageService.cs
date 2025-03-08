using GatewayService.Messages.Models.Requests;
using GatewayService.Messages.Models.Responses;

namespace GatewayService.Messages.Services;

public interface IDeleteMessageService
{
    Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequestDto request);
}