using GatewayService.Messages.Models;

namespace GatewayService.Messages.Services;

public interface ICreateMessageService
{
    Task SendMessageAsync(SendMessageRequestDto request);
}