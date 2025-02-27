using GatewayService.NewMessages.Models;

namespace GatewayService.NewMessages.Services;

public interface IMessageService
{
    Task SendMessageAsync(SendMessageRequest request);
}