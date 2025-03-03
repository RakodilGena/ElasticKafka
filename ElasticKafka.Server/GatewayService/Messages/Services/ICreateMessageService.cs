using GatewayService.Messages.Models.Requests;

namespace GatewayService.Messages.Services;

public interface ICreateMessageService
{
    Task SendMessageAsync(SendMessageRequestDto request);
}