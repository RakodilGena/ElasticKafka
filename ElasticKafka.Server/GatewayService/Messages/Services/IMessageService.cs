using GatewayService.Messages.Models;

namespace GatewayService.Messages.Services;

public interface IMessageService
{
    Task SendMessageAsync(SendMessageRequest request);
}