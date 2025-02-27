using GatewayService.NewMessages.Models;
using GatewayService.NewMessages.Services;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.NewMessages.Controllers;

public partial class MessageController
{
    [HttpPost]
    public async Task<ActionResult> SendMessageAsync(
        [FromBody] SendMessageRequest request,
        [FromServices] IMessageService service)
    {
        await service.SendMessageAsync(request);
        
        return Ok();
    }
}