using GatewayService.ExceptionFilters;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.NewMessages.Controllers;

[Route("api/messages")]
[GatewayExceptionFilter]
public sealed partial class MessageController : ControllerBase;